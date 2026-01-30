using Application.DTO.Places;
using Application.DTO.Posts;
using Application.DTO.Users;
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
            CreateMap<Post, PostDto>();
            CreateMap<Place, PlaceDto>();
        }
    }
}
