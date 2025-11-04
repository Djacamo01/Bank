using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lafise.API.data.model;
using Lafise.API.services.Accounts.Dto;
using Lafise.API.services.Clients.Dto;

namespace Lafise.API
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            // Mapeos de Client
            CreateMap<Client, ClientResponseDto>();
            
            
            // Mapeos de Account
            CreateMap<Client, ClientInfoDto>();
            CreateMap<Account, AccountDetailsDto>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.Client));
        }
    }
}