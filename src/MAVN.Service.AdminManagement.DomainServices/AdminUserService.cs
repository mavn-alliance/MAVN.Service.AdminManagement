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
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.CustomerProfile.Client.Models.Enums;
using Lykke.Service.CustomerProfile.Client.Models.Requests;
using Lykke.Service.CustomerProfile.Client.Models.Responses;
using MoreLinq;

namespace MAVN.Service.AdminManagement.DomainServices
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUsersRepository _adminUsersRepository;
        private readonly INotificationsService _notificationsService;
        private readonly ICredentialsClient _credentialsClient;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly IPermissionsService _permissionsService;
        private readonly IPermissionsCache _permissionsCache;
        private readonly ILog _log;

        public AdminUserService(
            IAdminUsersRepository adminUsersRepository,
            ICredentialsClient credentialsClient,
            ICustomerProfileClient customerProfileClient,
            IPermissionsService permissionsService,
            ILogFactory logFactory,
            INotificationsService notificationsService,
            IPermissionsCache permissionsCache)
        {
            _adminUsersRepository = adminUsersRepository;
            _credentialsClient = credentialsClient;
            _customerProfileClient = customerProfileClient;
            _permissionsService = permissionsService;
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

        public async Task<RegistrationResultModel> RegisterAsync(
            string email,
            string password,
            string firstName,
            string lastName,
            string phoneNumber,
            string company,
            string department,
            string jobTitle,
            IReadOnlyList<Permission> permissions)
        {
            var adminId = Guid.NewGuid().ToString();
            
            email = email.ToLower();
            
            CredentialsCreateResponse adminCredentialsCreationResult;

            try
            {
                adminCredentialsCreationResult = await _credentialsClient.Admins.CreateAsync(
                    new AdminCredentialsCreateRequest {Login = email, AdminId = adminId, Password = password});
            }
            catch (ClientApiException exception) when (exception.HttpStatusCode == HttpStatusCode.BadRequest)
            {
                return new RegistrationResultModel {Error = ServicesError.InvalidEmailOrPasswordFormat};
            }

            if (adminCredentialsCreationResult.Error == CredentialsError.LoginAlreadyExists)
                return new RegistrationResultModel {Error = ServicesError.AlreadyRegistered};

            var emailHash = GetHash(email);
            
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
                new AdminProfileRequest
                {
                    AdminId = Guid.Parse(adminId),
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    Company = company,
                    Department = department,
                    JobTitle = jobTitle,
                    PhoneNumber = phoneNumber
                });

            await _permissionsService.CreateOrUpdatePermissionsAsync(adminId, permissions);

            await _permissionsCache.SetAsync(adminId, permissions.ToList());

            if (adminProfileCreationResult.ErrorCode != AdminProfileErrorCodes.None)
            {
                _log.Error(message: "An error occurred while creating admin profile.",
                    context: $"adminUserId: {adminId}; error: {adminProfileCreationResult.ErrorCode}");
            }

            var fullName = $"{firstName} {lastName}";

            await _notificationsService.NotifyAdminCreatedAsync(adminId, email, email, password, fullName);

            return new RegistrationResultModel { Admin = new AdminUser
            {
                AdminUserId = adminId,
                Company = adminProfileCreationResult.Data.Company,
                Department = adminProfileCreationResult.Data.Department,
                Email = adminProfileCreationResult.Data.Email,
                FirstName = adminProfileCreationResult.Data.FirstName,
                LastName = adminProfileCreationResult.Data.LastName,
                JobTitle = adminProfileCreationResult.Data.JobTitle,
                PhoneNumber = adminProfileCreationResult.Data.PhoneNumber,
                Permissions = permissions.ToList(),
                RegisteredAt = registrationDateTime,
                UseDefaultPermissions = false,
                IsActive = true
            }};
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
            
            await _customerProfileClient.AdminProfiles.UpdateAsync(new AdminProfileRequest
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
            
            return new AdminUserResult
            {
                Error = AdminUserErrorCodes.None,
                Profile = new AdminUser
                {
                    AdminUserId = adminUserId,
                    Email = adminProfile.Data.Email,
                    IsActive = isActive,
                    Company = company,
                    Department = department,
                    FirstName = firstName,
                    JobTitle = jobTitle,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    RegisteredAt = adminUserEncrypted.RegisteredAt,
                    UseDefaultPermissions = adminUserEncrypted.UseDefaultPermissions,
                    Permissions = await GetPermissionsAsync(adminUserId)
                }
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
                var admin = new AdminUser
                {
                    AdminUserId = adminUserEncrypted.AdminUserId,
                    RegisteredAt = adminUserEncrypted.RegisteredAt,
                    IsActive = adminUserEncrypted.IsActive,
                    UseDefaultPermissions = adminUserEncrypted.UseDefaultPermissions
                };

                var adminId = Guid.Parse(adminUserEncrypted.AdminUserId);

                if (adminProfileMap.TryGetValue(adminId, out var adminProfile))
                {
                    admin.FirstName = adminProfile.FirstName;
                    admin.LastName = adminProfile.LastName;
                    admin.Email = adminProfile.Email;
                    admin.Company = adminProfile.Company;
                    admin.Department = adminProfile.Department;
                    admin.JobTitle = adminProfile.JobTitle;
                    admin.PhoneNumber = adminProfile.PhoneNumber;
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
            var admin = new AdminUser
            {
                AdminUserId = adminUserEncrypted.AdminUserId,
                RegisteredAt = adminUserEncrypted.RegisteredAt,
                IsActive = adminUserEncrypted.IsActive,
                UseDefaultPermissions = adminUserEncrypted.UseDefaultPermissions
            };

            var adminId = Guid.Parse(adminUserEncrypted.AdminUserId);

            var response = await _customerProfileClient.AdminProfiles.GetByIdAsync(adminId);

            if (response.ErrorCode != AdminProfileErrorCodes.None)
            {
                _log.Error(message: "An error occurred while getting admin profile.",
                    context: $"adminUserId: {adminUserEncrypted.AdminUserId}; error: {response.ErrorCode}");
            }
            else
            {
                admin.FirstName = response.Data.FirstName;
                admin.LastName = response.Data.LastName;
                admin.Email = response.Data.Email;
                admin.Company = response.Data.Company;
                admin.Department = response.Data.Department;
                admin.JobTitle = response.Data.JobTitle;
                admin.PhoneNumber = response.Data.PhoneNumber;
            }

            return admin;
        }

        private static string GetHash(string value)
        {
            return new Sha256HashingUtil().Sha256HashEncoding1252(value);
        }
    }
}
