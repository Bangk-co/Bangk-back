using AutoMapper;
using Bangk.Common.Dto;
using Bangk.Repositories.Entities;

namespace Bangk.Api.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Client, ClientUpdateInfoDto>();

        }
    }
}
