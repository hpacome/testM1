using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using System.Text;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Options;
using ASPA.DAL.Entity;
using Microsoft.AspNetCore.Identity;
using ASPA.DAL.Security;

namespace spa.DAL
{
    public class ApplicationDbContext : KeyApiAuthorizationDbContext<User, Role, int>
    {
        #region Constructor

        public ApplicationDbContext(DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {
        }

        #endregion Constructor

        #region Methods

        protected override void OnModelCreating(ModelBuilder
        modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Map Entity names to DB Table names
            modelBuilder.Entity<City>().ToTable("Cities");
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            //modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

            modelBuilder.Entity<User>(b =>
            {
                // Each User can have many UserClaims
                b.HasMany(e => e.Claims)
                    .WithOne()
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();
                b.ToTable("Users");
            });

        }

        #endregion Methods

        #region Properties

        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }

        #endregion Properties
    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);
            var storeOptions = Options.Create(new OperationalStoreOptions());
            return new ApplicationDbContext(builder.Options, storeOptions);
        }
    }
}
