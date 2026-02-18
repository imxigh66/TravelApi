using Amazon.S3;
using Amazon;
using Application.Auth.Register.CommandHandler;
using Application.Auth.Register.Commands;
using Application.Common.Behaviors;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using FluentValidation;
using Infrastructure;
using Infrastructure.ExternalServices.Email;
using Infrastructure.ExternalServices.S3;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System;
using System.Text;
using TravelApi.Middleware;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<TravelDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IApplicationDbContext>(sp =>
    sp.GetRequiredService<TravelDbContext>());

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.Configure<MailtrapOptions>(
    builder.Configuration.GetSection("Mailtrap"));
builder.Services.AddScoped<IEmailService, MailtrapService>();

// ===== AWS S3 Configuration =====
builder.Services.Configure<S3Options>(builder.Configuration.GetSection("AWS"));

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = builder.Configuration.GetSection("AWS").Get<S3Options>();

    var s3Config = new AmazonS3Config
    {
        RegionEndpoint = RegionEndpoint.GetBySystemName(config!.Region)
    };

    // Если используются Access Key и Secret Key
    if (!string.IsNullOrEmpty(config.AccessKeyId) && !string.IsNullOrEmpty(config.SecretAccessKey))
    {
        return new AmazonS3Client(config.AccessKeyId, config.SecretAccessKey, s3Config);
    }

    // Иначе используем IAM роли (для production на AWS)
    return new AmazonS3Client(s3Config);
});
builder.Services.AddScoped<IImageProcessingService, ImageProcessingService>();
builder.Services.AddScoped<IFileStorageService, S3FileStorageService>();

builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(RegisterCommandHandler).Assembly);
});

builder.Services.AddValidatorsFromAssembly(typeof(RegisterCommand).Assembly);

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// ============= JWT AUTHENTICATION =============
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Убрать 5-минутный запас
    };
});

builder.Services.AddAuthorization();

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")  // React dev server
              .AllowAnyMethod()                       // GET, POST, PUT, DELETE, etc.
              .AllowAnyHeader()                       // Authorization, Content-Type, etc.
              .AllowCredentials();                    // Cookies, Authorization headers
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
     
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TravelSocial API",
        Version = "v1"
    });

    // Добавить поддержку JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    
});

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
