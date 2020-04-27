using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Log;
using Falcon.Common;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using MAVN.Service.AdminManagement.Domain.Enums;
using MAVN.Service.AdminManagement.Domain.Models;
using MAVN.Service.AdminManagement.Domain.Repositories;
using MAVN.Service.AdminManagement.Domain.Services;
using Lykke.Service.Credentials.Client;
using Lykke.Service.Credentials.Client.Models.Requests;
using Lykke.Service.Credentials.Client.Models.Responses;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.CustomerProfile.Client.Models.Enums;
using MAVN.Service.CustomerProfile.Client.Models.Requests;
using MAVN.Service.CustomerProfile.Client.Models.Responses;
using MoreLinq;
using AutoMapper;
using MAVN.Service.AdminManagement.Domain.Models.Emails;
using Common;

namespace MAVN.Service.AdminManagement.DomainServices
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUsersRepository _adminUsersRepository;
        private readonly INotificationsService _notificationsService;
        private readonly ICredentialsClient _credentialsClient;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IEmailVerificationCodeRepository _emailVerificationCodeRepository;
        private readonly IPermissionsService _permissionsService;
        private readonly IMapper _mapper;
        private readonly IPermissionsCache _permissionsCache;
        private readonly ILog _log;

        public AdminUserService(
            IAdminUsersRepository adminUsersRepository,
            ICredentialsClient credentialsClient,
            ICustomerProfileClient customerProfileClient,
            IEmailVerificationCodeRepository emailVerificationCodeRepository,
            IPermissionsService permissionsService,
            ILogFactory logFactory,
            IMapper mapper,
            INotificationsService notificationsService,
            IPermissionsCache permissionsCache)
        {
            _adminUsersRepository = adminUsersRepository;
            _credentialsClient = credentialsClient;
            _customerProfileClient = customerProfileClient;
            _emailVerificationCodeRepository = emailVerificationCodeRepository;
            _permissionsService = permissionsService;
            _mapper = mapper;
            _notificationsService = notificationsService;
            _permissionsCache = permissionsCache;
            _log = logFactory.CreateLog(this);
        }

        public async Task<IReadOnlyList<AdminUser>> GetAllAsync()
        {
            var adminUsersEncrypted = await _adminUsersRepository.GetAllAsync();

            var adminUsers = await LoadSensitiveDataAsync(adminUsersEncrypted);
            
            foreach (var adminUser in adminUsers)
            {
                adminUser.Permissions = await GetPermissionsAsync(adminUser.AdminUserId);
            }

            return adminUsers;
        }

        public async Task<PaginatedAdminUserModel> GetPaginatedAsync(int currentPage, int pageSize, bool? active)
        {
            if (currentPage < 1)
                throw new ArgumentException("Current page can't be negative", nameof(currentPage));

            if (pageSize < 1)
                throw new ArgumentException("Page size can't be 0 or negative", nameof(pageSize));

            var skip = (currentPage - 1) * pageSize;
            var take = pageSize;

            var (adminUsersEncrypted, count) = await _adminUsersRepository.GetPaginatedAsync(skip, take, active);

            var adminUsers = await LoadSensitiveDataAsync(adminUsersEncrypted);

            foreach (var adminUser in adminUsers)
            {
                adminUser.Permissions = await GetPermissionsAsync(adminUser.AdminUserId);
            }
            
            return new PaginatedAdminUserModel
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                AdminUsers = adminUsers,
                TotalCount = count
            };
        }

        public async Task<AdminUserResult> GetByEmailAsync(string email, bool? active)
        {
            string emailHash = GetHash(email);

            var adminUserEncrypted = await _adminUsersRepository.GetByEmailAsync(email, active);

            if (adminUserEncrypted == null)
            {
                _log.Warning("Admin user not found", context: $"email: {emailHash}");

                return new AdminUserResult {Error = AdminUserErrorCodes.AdminUserDoesNotExist};
            }

            var profile = await LoadSensitiveDataAsync(adminUserEncrypted);

            profile.Permissions = await GetPermissionsAsync(adminUserEncrypted.AdminUserId);
            
            return new AdminUserResult {Profile = profile};
        }

        public async Task<AdminPasswordResetResult> ResetPasswordAsync(string adminUserId, string password)
        {
            var adminUserEncrypted = await _adminUsersRepository.GetAsync(adminUserId);

            if (adminUserEncrypted == null)
            {
                return new AdminPasswordResetResult
                {
                    Error = AdminPasswordResetErrorCode.AdminUserNotFound
                };
            }

            var adminProfile = await LoadSensitiveDataAsync(adminUserEncrypted);
            
            try
            {
                await _credentialsClient.Admins.ChangePasswordAsync(new AdminCredentialsUpdateRequest
                {
                    AdminId = adminUserEncrypted.AdminUserId,
                    Login = adminProfile.Email,
                    Password = password
                });
            }
            catch (ClientApiException exception) when (exception.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return new AdminPasswordResetResult
                {
                    Error = AdminPasswordResetErrorCode.InvalidPassword
                };
            }

            var fullName = $"{adminProfile.FirstName} {adminProfile.LastName}";

            await _notificationsService.NotifyAdminPasswordResetAsync(adminUserId, adminProfile.Email, adminProfile.Email, password, fullName);

            return new AdminPasswordResetResult
            {
                Error = AdminPasswordResetErrorCode.None,
                Profile = adminProfile
            };
        }

        public async Task<AdminUserResult> UpdatePermissionsAsync(string adminUserId, List<Permission> permissions)
        {
            var adminUserEncrypted = await _adminUsersRepository.GetAsync(adminUserId);

            if (adminUserEncrypted == null)
            {
                _log.Warning("Admin user not found", context: $"Id: {adminUserId}");

                return new AdminUserResult {Error = AdminUserErrorCodes.AdminUserDoesNotExist};
            }
            
            adminUserEncrypted.UseDefaultPermissions = false;
            
            await _permissionsService.CreateOrUpdatePermissionsAsync(adminUserId, permissions);

            var profile = await LoadSensitiveDataAsync(adminUserEncrypted);

            profile.Permissions = permissions;

            await _adminUsersRepository.TryUpdateAsync(adminUserEncrypted);

            await _permissionsCache.SetAsync(adminUserEncrypted.AdminUserId, permissions);
            
            return new AdminUserResult {Profile = profile};
        }

        public async Task<RegistrationResultModel> RegisterAsync(RegistrationRequestDto model)
        {
            var adminId = Guid.NewGuid().ToString();

            model.Email = model.Email.ToLower();
            
            CredentialsCreateResponse adminCredentialsCreationResult;

            try
            {
                adminCredentialsCreationResult = await _credentialsClient.Admins.CreateAsync(
                    new AdminCredentialsCreateRequest {Login = model.Email, AdminId = adminId, Password = model.Password});
            }
            catch (ClientApiException exception) when (exception.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return new RegistrationResultModel {Error = ServicesError.InvalidEmailOrPasswordFormat};
            }

            if (adminCredentialsCreationResult.Error == CredentialsError.LoginAlreadyExists)
                return new RegistrationResultModel {Error = ServicesError.AlreadyRegistered};

            var emailHash = GetHash(model.Email);
            
            if (adminCredentialsCreationResult.Error != CredentialsError.None)
            {
                const string errorMessage = "An error occurred while creating admin credentials.";
                
                _log.Error(errorMessage, context:
                    $"adminUserId: {adminId}; email: {emailHash}; error: {adminCredentialsCreationResult.Error}");

                throw new InvalidOperationException(errorMessage);
            }

            var registrationDateTime = DateTime.UtcNow;

            var result = await _adminUsersRepository.TryCreateAsync(
                new AdminUserEncrypted
                {
                    AdminUserId = adminId,
                    EmailHash = emailHash,
                    RegisteredAt = registrationDateTime,
                    IsActive = true,
                    UseDefaultPermissions = false
                });

            if (result)
            {
                _log.Info("Admin user created for an account.", context: $"adminUserId: {adminId}; email: {emailHash}");
            }
            else
            {
                _log.Warning("Trying to create a duplicate admin user.", context: $"email: {emailHash}");
                
                return new RegistrationResultModel {Error = ServicesError.AlreadyRegistered};
            }

            var adminProfileCreationResult = await _customerProfileClient.AdminProfiles.AddAsync(
                _mapper.Map<RegistrationRequestDto, AdminProfileRequest>(model,
                    opt => opt.AfterMap((src, dest) => { dest.AdminId = Guid.Parse(adminId); }))
                );

            await _permissionsService.CreateOrUpdatePermissionsAsync(adminId, model.Permissions);

            await _permissionsCache.SetAsync(adminId, model.Permissions.ToList());

            if (adminProfileCreationResult.ErrorCode != AdminProfileErrorCodes.None)
            {
                _log.Error(message: "An error occurred while creating admin profile.",
                    context: $"adminUserId: {adminId}; error: {adminProfileCreationResult.ErrorCode}");
            }

            #region email verification code

            var emailVerificationCode = Guid.NewGuid().ToString();

            var emailVerificationCodeEntity = await _emailVerificationCodeRepository.CreateOrUpdateAsync(
                adminId,
                emailVerificationCode);

            #endregion

            await _notificationsService.NotifyAdminCreatedAsync(new AdminCreatedEmailDto
            {
                AdminUserId = adminId,
                Email = model.Email,
                EmailVerificationCode = emailVerificationCode.ToBase64(),
                Password = model.Password,
                Name = $"{model.FirstName} {model.LastName}",
                Localization = model.Localization
            });

            _log.Info(message: "Successfully generated AdminCreatedEmail", context: adminId);

            var adminUser = _mapper.Map<AdminUser>(adminProfileCreationResult.Data);
            adminUser.AdminUserId = adminId;
            adminUser.Permissions = model.Permissions.ToList();
            adminUser.RegisteredAt = registrationDateTime;
            adminUser.UseDefaultPermissions = false;
            adminUser.IsActive = true;

            return new RegistrationResultModel { Admin = adminUser };
        }

        public async Task<AdminUserResult> GetByIdAsync(string adminId)
        {
            var adminUserEncrypted = await _adminUsersRepository.GetAsync(adminId);

            if (adminUserEncrypted == null)
            {
                _log.Warning("Admin user not found", context: $"Id: {adminId}");

                return new AdminUserResult {Error = AdminUserErrorCodes.AdminUserDoesNotExist};
            }

            var profile = await LoadSensitiveDataAsync(adminUserEncrypted);

            profile.Permissions = await GetPermissionsAsync(adminId);
            
            return new AdminUserResult {Profile = profile};
        }

        public async Task<AdminUserResult> UpdateDataAsync(
            string adminUserId,
            string company,
            string department,
            string firstName,
            string lastName,
            string jobTitle,
            string phoneNumber,
            bool isActive)
        {
            var adminUserEncrypted = await _adminUsersRepository.GetAsync(adminUserId);

            if (adminUserEncrypted == null)
            {
                return new AdminUserResult
                {
                    Error = AdminUserErrorCodes.AdminUserDoesNotExist
                };
            }

            adminUserEncrypted.IsActive = isActive;
            adminUserEncrypted.UseDefaultPermissions = false;

            await _adminUsersRepository.TryUpdateAsync(adminUserEncrypted);

            var adminProfile = await _customerProfileClient.AdminProfiles.GetByIdAsync(Guid.Parse(adminUserId));

            adminProfile = await _customerProfileClient.AdminProfiles.UpdateAsync(new AdminProfileRequest
            {
                AdminId = Guid.Parse(adminUserId),
                Email = adminProfile.Data.Email,
                Company = company,
                Department = department,
                FirstName = firstName,
                LastName = lastName,
                JobTitle = jobTitle,
                PhoneNumber = phoneNumber
            });

            var adminUser = _mapper.Map<AdminUser>(adminUserEncrypted);
            _mapper.Map(adminProfile.Data, adminUser);

            return new AdminUserResult
            {
                Error = AdminUserErrorCodes.None,
                Profile = adminUser
            };
        }

        public async Task<List<Permission>> GetPermissionsAsync(string adminId)
        {
            var (permissionsInCache, permissions) = await _permissionsCache.TryGetAsync(adminId);

            if (!permissionsInCache)
            {
                _log.Info("Populating cache.");

                var admin = await _adminUsersRepository.GetAsync(adminId);

                permissions = admin?.UseDefaultPermissions ?? true
                    ? null
                    : (await _permissionsService.GetPermissionsAsync(adminId)).ToList();

                await _permissionsCache.SetAsync(adminId, permissions);
            }
            else
            {
                _log.Info("Getting from cache.");
            }

            return permissions;
        }

        private async Task<IReadOnlyList<AdminUser>> LoadSensitiveDataAsync(
            IReadOnlyList<AdminUserEncrypted> adminUsersEncrypted)
        {
            var adminProfileMap = new Dictionary<Guid, AdminProfile>();

            foreach (var adminUsersEncryptedGroup in adminUsersEncrypted.Batch(10).ToList())
            {
                var identifiers = adminUsersEncryptedGroup
                    .Select(o => Guid.Parse(o.AdminUserId))
                    .ToArray();

                var profiles = await _customerProfileClient.AdminProfiles.GetAsync(identifiers);

                foreach (var profile in profiles)
                    adminProfileMap.Add(profile.AdminId, profile);
            }

            var admins = new List<AdminUser>();

            foreach (var adminUserEncrypted in adminUsersEncrypted)
            {
                var admin = _mapper.Map<AdminUser>(adminUserEncrypted);

                var adminId = Guid.Parse(adminUserEncrypted.AdminUserId);

                if (adminProfileMap.TryGetValue(adminId, out var adminProfile))
                {
                    _mapper.Map(adminProfile, admin);
                }
                else
                {
                    _log.Error(message: "Admin profile not found.",
                        context: $"adminUserId: {adminUserEncrypted.AdminUserId}");
                }

                admins.Add(admin);
            }

            return admins;
        }

        private async Task<AdminUser> LoadSensitiveDataAsync(AdminUserEncrypted adminUserEncrypted)
        {
            var admin = _mapper.Map<AdminUser>(adminUserEncrypted);

            var adminId = Guid.Parse(adminUserEncrypted.AdminUserId);

            var adminProfile = await _customerProfileClient.AdminProfiles.GetByIdAsync(adminId);

            if (adminProfile.ErrorCode != AdminProfileErrorCodes.None)
            {
                _log.Error(message: "An error occurred while getting admin profile.",
                    context: $"adminUserId: {adminUserEncrypted.AdminUserId}; error: {adminProfile.ErrorCode}");
            }
            else
            {
                _mapper.Map(adminProfile.Data, admin);
            }

            return admin;
        }

        private static string GetHash(string value)
        {
            return new Sha256HashingUtil().Sha256HashEncoding1252(value);
        }
    }
}
