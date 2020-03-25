using System.Linq;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.AdminManagement.Client.Models;
using Lykke.Service.AdminManagement.Client.Models.Enums;
using Lykke.Service.AdminManagement.Client.Models.Requests;
using Lykke.Service.AdminManagement.Domain.Enums;
using Lykke.Service.AdminManagement.Domain.Models;
using AdminUser = Lykke.Service.AdminManagement.Domain.Models.AdminUser;

namespace Lykke.Service.AdminManagement
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AllowNullCollections = true;

            CreateMap<AdminPasswordResetResult, ResetPasswordResponseModel>();
            CreateMap<AdminPasswordResetErrorCode, ResetPasswordErrorCodes>();
            CreateMap<AuthResultModel, AuthenticateResponseModel>();
            CreateMap<PasswordUpdateError, ChangePasswordErrorCodes>();
            CreateMap<RegistrationResultModel, RegistrationResponseModel>();
            CreateMap<Domain.Models.AdminUser, Client.Models.AdminUser>();
            CreateMap<PaginatedAdminUserModel, PaginatedAdminUserResponseModel>();
            CreateMap<AdminUserResult, AdminUserResponseModel>()
                .ForMember(c => c.Profile, a => a.MapFrom(x => x.Profile))
                .ForMember(c => c.Error, a => a.MapFrom(x => x.Error));
            CreateMap<AdminUserResponseErrorCodes, AdminUserErrorCodes>();
            
            CreateMap<AdminPermission, Permission>();
            CreateMap<Permission, AdminPermission>();
            
            CreateMap<PermissionLevel, AdminPermissionLevel>();
            CreateMap<AdminPermissionLevel, PermissionLevel>();
        }
    }
}
