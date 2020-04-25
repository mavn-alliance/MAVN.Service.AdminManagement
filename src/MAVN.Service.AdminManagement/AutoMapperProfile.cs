using AutoMapper;
using JetBrains.Annotations;
using MAVN.Service.AdminManagement.Client.Models;
using MAVN.Service.AdminManagement.Client.Models.Enums;
using MAVN.Service.AdminManagement.Client.Models.Responses.Verification;
using MAVN.Service.AdminManagement.Domain.Enums;
using MAVN.Service.AdminManagement.Domain.Models;
using MAVN.Service.AdminManagement.Domain.Models.Verification;
using MAVN.Service.CustomerProfile.Client.Models.Requests;
using MAVN.Service.CustomerProfile.Client.Models.Responses;

namespace MAVN.Service.AdminManagement
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AllowNullCollections = true;

            // Auth and password
            CreateMap<AdminPasswordResetResult, ResetPasswordResponseModel>();
            CreateMap<AdminPasswordResetErrorCode, ResetPasswordErrorCodes>();
            CreateMap<AuthResultModel, AuthenticateResponseModel>();
            CreateMap<PasswordUpdateError, ChangePasswordErrorCodes>();

            // Registration
            CreateMap<RegistrationRequestModel, RegistrationRequestDto>();
            CreateMap<RegistrationRequestDto, AdminProfileRequest>()
                .ForMember(dest => dest.AdminId, opt => opt.Ignore());
            CreateMap<RegistrationResultModel, RegistrationResponseModel>();
            CreateMap<Client.Models.Enums.Localization, Domain.Enums.Localization>();
            CreateMap<ConfirmVerificationCodeResultModel, VerificationCodeConfirmationResponseModel>();

            #region AdminUser

            CreateMap<MAVN.Service.CustomerProfile.Client.Models.Responses.AdminProfile, Domain.Models.AdminUser>(MemberList.None);
            CreateMap<AdminUserEncrypted, Domain.Models.AdminUser>(MemberList.None);
            CreateMap<Domain.Models.AdminUser, Client.Models.AdminUser>();
            CreateMap<PaginatedAdminUserModel, PaginatedAdminUserResponseModel>();
            CreateMap<AdminUserResult, AdminUserResponseModel>();
            CreateMap<AdminUserResponseErrorCodes, AdminUserErrorCodes>();

            #endregion

            // Permission
            CreateMap<AdminPermission, Permission>();
            CreateMap<Permission, AdminPermission>();
            CreateMap<PermissionLevel, AdminPermissionLevel>();
            CreateMap<AdminPermissionLevel, PermissionLevel>();
        }
    }
}
