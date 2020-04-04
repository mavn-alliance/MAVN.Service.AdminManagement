using AutoMapper;
using MAVN.Service.AdminManagement.Domain.Models;
using MAVN.Service.AdminManagement.MsSqlRepositories.Entities;

namespace MAVN.Service.AdminManagement.MsSqlRepositories
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AdminUserEncrypted, AdminUserEntity>(MemberList.Source)
                .ForMember(x => x.IsDisabled, x => x.MapFrom(y => !y.IsActive))
                .ForMember(x => x.UseCustomPermissions, x => x.MapFrom(y => !y.UseDefaultPermissions));
            CreateMap<AdminUserEntity, AdminUserEncrypted>(MemberList.Destination)
                .ForMember(x => x.IsActive, x => x.MapFrom(y => !y.IsDisabled))
                .ForMember(x => x.UseDefaultPermissions, x => x.MapFrom(y => !y.UseCustomPermissions));
        }
    }
}
