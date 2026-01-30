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
            CreateMap<RegisterCommand, User>();
            CreateMap<Post, PostDto>();
            CreateMap<CreatePostCommand, Post>();
            CreateMap<Place, PlaceDto>();
            CreateMap<CreatePlaceCommand, Place>();

        }
    }
}
