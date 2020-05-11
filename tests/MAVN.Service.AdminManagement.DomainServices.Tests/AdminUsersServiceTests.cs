using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Common;
using Lykke.Logs;
using MAVN.Service.AdminManagement.Domain.Enums;
using MAVN.Service.AdminManagement.Domain.Models;
using MAVN.Service.AdminManagement.Domain.Repositories;
using MAVN.Service.AdminManagement.Domain.Services;
using MAVN.Service.Credentials.Client;
using MAVN.Service.Credentials.Client.Models.Requests;
using MAVN.Service.Credentials.Client.Models.Responses;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.CustomerProfile.Client.Models.Enums;
using MAVN.Service.CustomerProfile.Client.Models.Requests;
using MAVN.Service.CustomerProfile.Client.Models.Responses;
using Moq;
using Xunit;
using AutoMapper;

namespace MAVN.Service.AdminManagement.DomainServices.Tests
{
    public class AdminUsersServiceTests
    {
        private readonly Mock<IEmailVerificationCodeRepository> _emailVerificationCodeRepositoryMock =
            new Mock<IEmailVerificationCodeRepository>();

        private readonly Mock<IAdminUsersRepository> _adminUsersRepositoryMock =
            new Mock<IAdminUsersRepository>();

        private readonly Mock<ICredentialsClient> _credentialsClientMock =
            new Mock<ICredentialsClient>();

        private readonly Mock<ICustomerProfileClient> _customerProfileClientMock =
            new Mock<ICustomerProfileClient>();

        private readonly Mock<IPermissionsService> _permissionsServiceMock = 
            new Mock<IPermissionsService>();

        private readonly Mock<INotificationsService> _notificationsServiceMock = 
            new Mock<INotificationsService>();

        private readonly Mock<IPermissionsCache> _permissionsCacheMock = 
            new Mock<IPermissionsCache>();

        private readonly IAdminUserService _service;

        public AdminUsersServiceTests()
        {
            _adminUsersRepositoryMock.Setup(o => o.TryCreateAsync(It.IsAny<AdminUserEncrypted>()))
                .ReturnsAsync((AdminUserEncrypted adminUserEncrypted) => true);

            _credentialsClientMock.Setup(o => o.Admins.CreateAsync(It.IsAny<AdminCredentialsCreateRequest>()))
                .ReturnsAsync((AdminCredentialsCreateRequest request) =>
                    new CredentialsCreateResponse {Error = CredentialsError.None});

            _customerProfileClientMock.Setup(o => o.AdminProfiles.AddAsync(It.IsAny<AdminProfileRequest>()))
                .ReturnsAsync((AdminProfileRequest request) => new AdminProfileResponse
                {
                    Data = new AdminProfile
                    {
                        AdminId = request.AdminId,
                        Email = request.Email,
                        FirstName = request.FirstName,
                        LastName = request.LastName
                    },
                    ErrorCode = AdminProfileErrorCodes.None
                });

            var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfile()); });
            var mapper = mockMapper.CreateMapper();

            _service = new AdminUserService(
                _adminUsersRepositoryMock.Object,
                _credentialsClientMock.Object,
                _customerProfileClientMock.Object,
                _emailVerificationCodeRepositoryMock.Object,
                _permissionsServiceMock.Object,
                EmptyLogFactory.Instance,
                mapper,
                _notificationsServiceMock.Object,
                _permissionsCacheMock.Object);
        }

        [Fact]
        public async Task Register_New_Admin_And_Return_Identifier()
        {
            // act

            var result = await _service.RegisterAsync(GetRegisterRequest());

            // assert

            Assert.NotEqual(Guid.Empty.ToString(), result.Admin.AdminUserId);
        }

        [Fact]
        public async Task Create_Credentials_While_Registering_New_Admin()
        {
            // arrange

            const string email = "email@email.com";
            const string password = "password";

            var model = GetRegisterRequest();
            model.Email = email;
            model.Password = password;

            // act

            await _service.RegisterAsync(model);

            // assert

            _credentialsClientMock.Verify(o => o.Admins.CreateAsync(It.Is<AdminCredentialsCreateRequest>(
                request => request.Login == email && request.Password == password)));
        }

        [Fact]
        public async Task Save_Encrypted_Data_While_Registering_New_Admin()
        {
            // arrange

            const string email = "email@email.com";
            string emailHash = new Sha256HashingUtil().Sha256HashEncoding1252(email);

            var model = GetRegisterRequest();
            model.Email = email;

            // act

            await _service.RegisterAsync(model);

            // assert

            _adminUsersRepositoryMock.Verify(o => o.TryCreateAsync(It.Is<AdminUserEncrypted>(
                adminUserEncrypted => adminUserEncrypted.EmailHash == emailHash)));
        }

        [Fact]
        public async Task Create_Profile_While_Registering_New_Admin()
        {
            // arrange

            const string email = "email@email.com";
            const string firstName = "first name";
            const string lastName = "last name";

            var model = GetRegisterRequest();
            model.Email = email;
            model.FirstName = firstName;
            model.LastName = lastName;

            // act

            await _service.RegisterAsync(model);

            // assert

            _customerProfileClientMock.Verify(o => o.AdminProfiles.AddAsync(It.Is<AdminProfileRequest>(
                request => request.Email == email && request.FirstName == firstName && request.LastName == lastName)));
        }

        [Fact]
        public async Task Return_Error_Login_Already_Exists_While_Registering_Admin_With_Same_Email()
        {
            // arrange

            _credentialsClientMock.Setup(o => o.Admins.CreateAsync(It.IsAny<AdminCredentialsCreateRequest>()))
                .ReturnsAsync(new CredentialsCreateResponse {Error = CredentialsError.LoginAlreadyExists});

            // act

            var result = await _service.RegisterAsync(GetRegisterRequest());

            // assert

            Assert.Equal(ServicesError.AlreadyRegistered, result.Error);
        }

        private RegistrationRequestDto GetRegisterRequest()
        {
            return new RegistrationRequestDto
            {
                Email = "email@email.com",
                Password = "password",
                FirstName = "first name",
                LastName = "last name",
                PhoneNumber = "phone_number",
                Company = "company",
                Department = "department",
                JobTitle = "jobTitle",
                Permissions = new List<Permission>(),
                Localization = Localization.En
            };
        }
    }
}
