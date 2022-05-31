using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using ASPA.UI.Controllers;
using Xunit;
using spa.DAL;
using ASPA.DAL.Security;

namespace ASPA.UI.Tests
{
    public class SeedController_Tests
    {
        /// <summary>
        /// Test the CreateDefaultUsers() method
        /// </summary>
        [Fact]
        public async void CreateDefaultUsers()
        {
            #region Arrange
            // create the option instances required by the
            // ApplicationDbContext
            var options = new
            DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "WorldCities")
            .Options;
            var storeOptions = Options.Create(new
            OperationalStoreOptions());

            // create a IWebHost environment mock instance
            var mockEnv = new Mock<IWebHostEnvironment>().Object;

            // define the variables for the users we want to test
            User user_Admin = null;
            User user_User = null;
            User user_NotExisting = null;
            #endregion

            #region Act
            // create a ApplicationDbContext instance using the
            // in-memory DB
            using (var context = new ApplicationDbContext(options,
            storeOptions))
            {
                // create a RoleManager instance
                var roleStore = new RoleStore<Role, DbContext, int>(context);
                var roleManager = new RoleManager<Role>(
                        roleStore,
                        new IRoleValidator<Role>[0],
                        new UpperInvariantLookupNormalizer(),
                        new Mock<IdentityErrorDescriber>().Object,
                        new Mock<ILogger<RoleManager<Role>>>(
                    ).Object);

                // create a UserManager instance
                var userStore = new UserStore<User, Role, DbContext, int>(context);
                var userManager = new UserManager<User>(
                        userStore,
                        new Mock<IOptions<IdentityOptions>>().Object,
                        new Mock<IPasswordHasher<User>>().Object,
                        new IUserValidator<User>[0],
                        new IPasswordValidator<User>[0],
                        new UpperInvariantLookupNormalizer(),
                        new Mock<IdentityErrorDescriber>().Object,
                        new Mock<IServiceProvider>().Object,
                        new Mock<ILogger<UserManager<User>>>(
                    ).Object);

                // create a SeedController instance
                var controller = new SeedController(
                    context,
                    roleManager,
                    userManager,
                    mockEnv
                );

                // execute the SeedController's CreateDefaultUsers()
                // method to create the default users (and roles)
                await controller.CreateDefaultUsers();

                // retrieve the users
                user_Admin = await userManager.FindByEmailAsync("admin@email.com");
                user_User = await userManager.FindByEmailAsync("user@email.com");
                user_NotExisting = await userManager.FindByEmailAsync("notexisting@email.com");
            }
            #endregion

            #region Assert
            Assert.True(
            user_Admin != null
            && user_User != null
            && user_NotExisting == null
            );
            #endregion
        }
    }
}
