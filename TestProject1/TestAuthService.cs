using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CorpMessengerBackend.HttpObjects;
using CorpMessengerBackend.Models;
using CorpMessengerBackend.Services;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;

namespace TestProject1
{
    [TestClass]
    public class TestAuthService
    {
        private readonly Random _rnd = new Random();
        private readonly Mock<ICriptographyProvider> _cryptoMock;
        private readonly Mock<IDateTimeService> _dateTimeMock;
        private readonly Mock<IUserAuthProvider> _authProviderMock;
        private readonly LocalAuthService _sut;
        private readonly DateTime _dateTimeMin = DateTime.MinValue;

        public TestAuthService()
        {
            _cryptoMock = new();
            _dateTimeMock = new ();
            _authProviderMock = new Mock<IUserAuthProvider>();
            _sut = new(_cryptoMock.Object, _dateTimeMock.Object, _authProviderMock.Object);
        }

        [TestMethod]
        public void TestSignInEmail_ShouldCreateAndReturnAuth_WhenCredentialsAreOk()
        {
            // Arrange
            var creds = new Credentials()
            {
                DeviceId = "1",
                Email = "asd@asd.com",
                Password = "123qwe"
            };

            var user = new User()
            {
                Deleted = false,
                FirstName = "Name",
                SecondName = "Surname",
                Email = creds.Email,
                UserId = 1,
                DepartmentId = 1,
                Modified = _dateTimeMin
            };
            
            var opts = new DbContextOptionsBuilder<AppDataContext>()
                .UseInMemoryDatabase(databaseName: "TestSignInEmail_ShouldCreateAndReturnAuth_WhenCredentialsAreOk")
                .Options;

            using var context = new AppDataContext(opts);
            context.Departments.Add(new()
            {
                DepartmentId = 1,
                DepartmentName = "test",
                Modified = _dateTimeMin
            });
            
            context.Users.Add(user);

            context.UserSecrets.Add(new ()
            {
                Secret = "123",
                UserId = 1,
                Id = 1,
                User = user
            });

            context.SaveChanges();

            _cryptoMock.Setup(m => m.CheckPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            _cryptoMock.Setup(m => m.GenerateNewToken()).Returns("tokentokentoken1111111111111111");
            _cryptoMock.Invocations.Clear();
            _dateTimeMock.Setup(m => m.CurrentDateTime).Returns(_dateTimeMin);
            _dateTimeMock.Setup(m => m.MinValidTokenDateTime).Returns(_dateTimeMin);
            
            
            // Act
            var result = _sut.SignInEmail(context, creds);
            

            // Assert
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(user.UserId, result.Result.UserId);
            Assert.IsNotNull(result.Result.AuthToken);
            Assert.AreEqual(1, context.Auths.Count());
            _cryptoMock.Verify(m=> m.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _cryptoMock.Verify(m=> m.GenerateNewToken(), Times.Once);
        }

        [TestMethod]
        public void TestSignInEmail_ShouldReturnNull_WhenEmailIsFake()
        {
            // Arrange 
            var creds = new Credentials()
            {
                DeviceId = "1",
                Email = "asd@asd.com",
                Password = "123qwe"
            };

            var user = new User()
            {
                Deleted = false,
                FirstName = "Name",
                SecondName = "Surname",
                Email = "fake@email.com",
                UserId = 1,
                DepartmentId = 1,
                Modified = _dateTimeMin
            };
            
            var opts = new DbContextOptionsBuilder<AppDataContext>()
                .UseInMemoryDatabase(databaseName: "TestSignInEmail_ShouldReturnNull_WhenEmailIsFake")
                .Options;

            using var context = new AppDataContext(opts);
            context.Departments.Add(new()
            {
                DepartmentId = 1,
                DepartmentName = "test",
                Modified = _dateTimeMin
            });
            
            context.Users.Add(user);

            context.UserSecrets.Add(new ()
            {
                Secret = "123",
                UserId = 1,
                Id = 1,
                User = user
            });

            context.SaveChanges();

            _cryptoMock.Setup(m => m.CheckPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            _cryptoMock.Setup(m => m.GenerateNewToken()).Returns("tokentokentoken1111111111111111");
            _cryptoMock.Invocations.Clear();
            _dateTimeMock.Setup(m => m.CurrentDateTime).Returns(_dateTimeMin);
            _dateTimeMock.Setup(m => m.MinValidTokenDateTime).Returns(_dateTimeMin);
            
            
            // Act
            var result = _sut.SignInEmail(context, creds);
            
            
            // Assert
            Assert.IsNull(result.Result);
            _cryptoMock.Verify(m=> m.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _cryptoMock.Verify(m=> m.HashPassword(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
            _cryptoMock.Verify(m=> m.GenerateNewToken(), Times.Never);
            Assert.AreEqual(0, context.Auths.Count());
        }

        [TestMethod]
        public void TestSignInEmail_ShouldReturnNull_WhenPasswordIsFake()
        {
            // Arrange 
            var creds = new Credentials()
            {
                DeviceId = "1",
                Email = "asd@asd.com",
                Password = "123qwe"
            };

            var user = new User()
            {
                Deleted = false,
                FirstName = "Name",
                SecondName = "Surname",
                Email = creds.Email,
                UserId = 1,
                DepartmentId = 1,
                Modified = _dateTimeMin
            };
            
            var opts = new DbContextOptionsBuilder<AppDataContext>()
                .UseInMemoryDatabase(databaseName: "TestSignInEmail_ShouldReturnNull_WhenPasswordIsFake")
                .Options;

            using var context = new AppDataContext(opts);
            context.Departments.Add(new()
            {
                DepartmentId = 1,
                DepartmentName = "test",
                Modified = _dateTimeMin
            });
            
            context.Users.Add(user);

            context.UserSecrets.Add(new ()
            {
                Secret = "123",
                UserId = 1,
                Id = 1,
                User = user
            });

            context.SaveChanges();

            _cryptoMock.Setup(m => m.CheckPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);
            _cryptoMock.Setup(m => m.GenerateNewToken()).Returns("tokentokentoken1111111111111111");
            _cryptoMock.Invocations.Clear();
            _dateTimeMock.Setup(m => m.CurrentDateTime).Returns(_dateTimeMin);
            _dateTimeMock.Setup(m => m.MinValidTokenDateTime).Returns(_dateTimeMin);
            
            
            // Act
            var result = _sut.SignInEmail(context, creds);
            
            
            // Assert
            Assert.IsNull(result.Result);
            _cryptoMock.Verify(m=> m.CheckPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _cryptoMock.Verify(m=> m.HashPassword(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
            _cryptoMock.Verify(m=> m.GenerateNewToken(), Times.Never);
            Assert.AreEqual(0, context.Auths.Count());
        }

        [TestMethod]
        public void TestSignOut_ShouldDeleteAuth_WhenCalled()
        {
            // Arrange 
            var creds = new Credentials()
            {
                DeviceId = "1",
                Email = "asd@asd.com",
                Password = "123qwe",
                Token = "3213213213213212131232131321"
            };
            
            var opts = new DbContextOptionsBuilder<AppDataContext>()
                .UseInMemoryDatabase(databaseName: "TestSignOut_ShouldDeleteAuth_WhenCalled")
                .Options;

            using var context = new AppDataContext(opts);

            context.Auths.Add(new()
            {
                AuthId = new Guid(),
                AuthToken = creds.Token,
                DeviceId = creds.DeviceId,
                UserId = 1,
                Modified = _dateTimeMin
            });

            context.SaveChanges();
            _authProviderMock.Setup(m => m.GetToken()).Returns(creds.Token);
            
            
            // Act
            var result = _sut.SignOut(context);
            
            
            // Assert
            Assert.AreEqual(true, result.Result);
            Assert.AreEqual(0, context.Auths.Count());
        }

        [TestMethod]
        public void TestCheckUserAuth_ShouldReturnUserId_WhenTokenIsOk()
        {
            // Arrange 
            var creds = new Credentials()
            {
                DeviceId = "1",
                Email = "asd@asd.com",
                Password = "123qwe",
                Token = "3213213213213212131232131321"
            };
            
            var user = new User()
            {
                Deleted = false,
                FirstName = "Name",
                SecondName = "Surname",
                Email = creds.Email,
                UserId = 1,
                DepartmentId = 1,
                Modified = _dateTimeMin
            };
            
            var opts = new DbContextOptionsBuilder<AppDataContext>()
                .UseInMemoryDatabase(databaseName: "TestCheckUserAuth_ShouldReturnUserId_WhenTokenIsOk")
                .Options;

            using var context = new AppDataContext(opts);

            context.Auths.Add(new()
            {
                AuthId = new Guid(),
                AuthToken = creds.Token,
                DeviceId = creds.DeviceId,
                UserId = 1,
                Modified = _dateTimeMin
            });
            context.Users.Add(user);

            context.SaveChanges();
            
            _dateTimeMock.Setup(m => m.CurrentDateTime).Returns(_dateTimeMin);
            _dateTimeMock.Setup(m => m.MinValidTokenDateTime).Returns(_dateTimeMin);
            _authProviderMock.Setup(m => m.GetToken()).Returns(creds.Token);
            
            
            // Act
            var result = _sut.CheckUserAuth(context, creds.Token);
            
            
            // Assert
            Assert.AreEqual(user.UserId, result);
            Assert.AreEqual(1, context.Auths.Count());
        }
        
        
        [TestMethod]
        public void TestCheckUserAuth_ShouldReturnZero_WhenTokenIsOutdated()
        {
            // Arrange 
            var creds = new Credentials()
            {
                DeviceId = "1",
                Email = "asd@asd.com",
                Password = "123qwe",
                Token = "3213213213213212131232131321"
            };
            
            var user = new User()
            {
                Deleted = false,
                FirstName = "Name",
                SecondName = "Surname",
                Email = creds.Email,
                UserId = 1,
                DepartmentId = 1,
                Modified = _dateTimeMin
            };
            
            var opts = new DbContextOptionsBuilder<AppDataContext>()
                .UseInMemoryDatabase(databaseName: "TestCheckUserAuth_ShouldReturnZero_WhenTokenIsOutdated")
                .Options;

            using var context = new AppDataContext(opts);

            context.Auths.Add(new()
            {
                AuthId = new Guid(),
                AuthToken = creds.Token,
                DeviceId = creds.DeviceId,
                UserId = 1,
                Modified = _dateTimeMin
            });
            context.Users.Add(user);

            context.SaveChanges();
            
            _dateTimeMock.Setup(m => m.CurrentDateTime).Returns(_dateTimeMin.AddDays(8));
            _dateTimeMock.Setup(m => m.MinValidTokenDateTime).Returns(_dateTimeMin.AddDays(8));
            _authProviderMock.Setup(m => m.GetToken()).Returns(creds.Token);
            
            
            // Act
            var result = _sut.CheckUserAuth(context, creds.Token);
            
            
            // Assert
            Assert.AreEqual(0, result);
        }


        [TestMethod]
        public async Task TestRenewAuth_ShouldReturnUserId_WhenTokenIsOk()
        {
            var creds = new Credentials()
            {
                DeviceId = "1",
                Email = "asd@asd.com",
                Password = "123qwe",
                Token = "3213213213213212131232131321"
            };
            
            var user = new User()
            {
                Deleted = false,
                FirstName = "Name",
                SecondName = "Surname",
                Email = creds.Email,
                UserId = 1,
                DepartmentId = 1,
                Modified = _dateTimeMin
            };
            
            var opts = new DbContextOptionsBuilder<AppDataContext>()
                .UseInMemoryDatabase(databaseName: "TestCRenewAuth_ShouldReturnUserId_WhenTokenIsOk")
                .Options;

            await using var context = new AppDataContext(opts);

            context.Auths.Add(new()
            {
                AuthId = new Guid(),
                AuthToken = creds.Token,
                DeviceId = creds.DeviceId,
                UserId = 1,
                Modified = _dateTimeMin
            });
            context.Users.Add(user);

            await context.SaveChangesAsync();
            
            _dateTimeMock.Setup(m => m.CurrentDateTime).Returns(_dateTimeMin);
            _dateTimeMock.Setup(m => m.MinValidTokenDateTime).Returns(_dateTimeMin);
            var newToken = "tokentokentoken1111111111111111";
            _cryptoMock.Setup(m => m.GenerateNewToken()).Returns(newToken);
            _authProviderMock.Setup(m => m.GetToken()).Returns(creds.Token);
            
            // Act
            var result = await _sut.RenewAuth(context, creds);
            
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(creds.Token, result.AuthToken);
            Assert.AreEqual(newToken, result.AuthToken);
            Assert.AreEqual(1, context.Auths.Count());
        }

        [TestMethod]
        public async Task TestRenewAuth_ShouldReturnNull_WhenTokenIsFake()
        {
            var creds = new Credentials()
            {
                DeviceId = "1",
                Email = "asd@asd.com",
                Password = "123qwe",
                Token = "3213213213213212131232131321"
            };
            
            var user = new User()
            {
                Deleted = false,
                FirstName = "Name",
                SecondName = "Surname",
                Email = creds.Email,
                UserId = 1,
                DepartmentId = 1,
                Modified = _dateTimeMin
            };
            
            var opts = new DbContextOptionsBuilder<AppDataContext>()
                .UseInMemoryDatabase(databaseName: "TestCRenewAuth_ShouldReturnNull_WhenTokenIsFake")
                .Options;

            await using var context = new AppDataContext(opts);

            context.Auths.Add(new()
            {
                AuthId = new Guid(),
                AuthToken = "sfdsdfasfasfasdfasfdasf",
                DeviceId = creds.DeviceId,
                UserId = 1,
                Modified = _dateTimeMin
            });
            context.Users.Add(user);

            await context.SaveChangesAsync();
            
            _dateTimeMock.Setup(m => m.CurrentDateTime).Returns(_dateTimeMin);
            _dateTimeMock.Setup(m => m.MinValidTokenDateTime).Returns(_dateTimeMin);
            var newToken = "tokentokentoken1111111111111111";
            _cryptoMock.Setup(m => m.GenerateNewToken()).Returns(newToken);
            _authProviderMock.Setup(m => m.GetToken()).Returns(creds.Token);
            
            // Act
            var result = await _sut.RenewAuth(context, creds);
            
            
            // Assert
            Assert.IsNull(result);
        }
    }
}
