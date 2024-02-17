using AutoMapper;
using GooNet.Application.DTOs.Responses;
using GooNet.Domain.Entities;

namespace GooNet.Application.AutoMapper;

public class DomainToDtoMappingProfile : Profile
{
    public DomainToDtoMappingProfile()
    {
        CreateMap<Site, SiteResponse>();
    }
}