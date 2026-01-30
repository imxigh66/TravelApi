using Application.Auth.Register.CommandHandler;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<TravelDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IApplicationDbContext>(sp =>
    sp.GetRequiredService<TravelDbContext>());

builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(RegisterCommandHandler).Assembly);
});

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
