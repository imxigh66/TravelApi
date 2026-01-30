using Application.Common.Interfaces;
using Application.Common.Models;
using Application.DTO.Places;
using Application.DTO.Posts;
using Application.Places.Queries;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Places.QueryHandler
{
    public class GetAllPlacesQueryHandler : IRequestHandler<GetAllPlacesQuery, PaginatedList<PlaceDto>> {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        public GetAllPlacesQueryHandler(IApplicationDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<PaginatedList<PlaceDto>> Handle(GetAllPlacesQuery request, CancellationToken cancellationToken)
        {

            var placeQuery = _context.Places
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .ProjectTo<PlaceDto>(_mapper.ConfigurationProvider);

            
            return await PaginatedList<PlaceDto>.CreateAsync(
                placeQuery,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );

        }
    }
}
