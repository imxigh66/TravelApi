using Application.Auth.Register.Commands;
using Application.DTO.Places;
using Application.DTO.Posts;
using Application.DTO.Users;
using Application.Places.Commands;
using Application.Posts.Commands;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterCommand, User>()
                 .ForMember(dest => dest.AccountType, opt => opt.MapFrom(src => src.AccountType))
            .ForMember(dest => dest.TravelInterest, opt => opt.MapFrom(src => src.TravelInterest))
            .ForMember(dest => dest.TravelStyle, opt => opt.MapFrom(src => src.TravelStyle))
            .ForMember(dest => dest.Country, opt => opt.Ignore()) 
            .ForMember(dest => dest.City, opt => opt.Ignore());

            CreateMap<Post, PostDto>();
            CreateMap<CreatePostCommand, Post>();
            CreateMap<Place, PlaceDto>();
            CreateMap<CreatePlaceCommand, Place>();
            CreateMap<User, PersonalProfileDto>();
            CreateMap<User, BusinessProfileDto>();

        }
    }
}
