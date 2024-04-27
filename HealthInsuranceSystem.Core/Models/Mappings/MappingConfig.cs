using AutoMapper;

using HealthInsuranceSystem.Core.Models.Domain;
using HealthInsuranceSystem.Core.Models.DTO.ClaimsDto;
using HealthInsuranceSystem.Core.Models.DTO.UserDto;
using HealthInsuranceSystem.Core.Security;

namespace HealthInsuranceSystem.Core.Models.Mappings
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<AddClaimDto, Claim>().ReverseMap();
            CreateMap<AddUserDto, User>().ReverseMap();
            CreateMap<UpdateClaimDto, Claim>().ReverseMap();
            CreateMap<GetUserDto, User>().ReverseMap();

            CreateMap<User, IdentityUser>();
        }
    }
}