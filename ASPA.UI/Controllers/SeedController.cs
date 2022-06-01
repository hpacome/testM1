using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPA.DAL.Entity;
using ASPA.DAL.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using spa.DAL;

namespace ASPA.UI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;

        public SeedController(
            ApplicationDbContext context,
            RoleManager<Role> roleManager,
            UserManager<User> userManager,
            IWebHostEnvironment env)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult> CreateDefaultUsers()
        {
            // setup the default role names
            string role_RegisteredUser = "RegisteredUser";
            string role_Administrator = "Administrator";

            // create the default roles (if they doesn't exist yet)
            if (await _roleManager.FindByNameAsync(role_RegisteredUser) == null)
                await _roleManager.CreateAsync(new Role()
                {
                    Name = role_RegisteredUser
                });

            if (await _roleManager.FindByNameAsync(role_Administrator) == null)
                await _roleManager.CreateAsync(new Role() { 
                    Name = role_Administrator
                });

            // create a list to track the newly added users
            var addedUserList = new List<User>();

            // check if the admin user already exist
            var email_Admin = "admin@email.com";
            if (await _userManager.FindByNameAsync(email_Admin) == null)
            {
                // create a new admin ApplicationUser account
                var user_Admin = new User()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_Admin,
                    Email = email_Admin,
                };

                // insert the admin user into the DB
                await _userManager.CreateAsync(user_Admin, "MySecr3t$");

                // assign the "RegisteredUser" and "Administrator" roles
                await _userManager.AddToRoleAsync(user_Admin,
                role_RegisteredUser);
                await _userManager.AddToRoleAsync(user_Admin,
                role_Administrator);

                // confirm the e-mail and remove lockout
                user_Admin.EmailConfirmed = true;
                user_Admin.LockoutEnabled = false;

                // add the admin user to the added users list
                addedUserList.Add(user_Admin);
            }

            // check if the standard user already exist
            var email_User = "user@email.com";
            if (await _userManager.FindByNameAsync(email_User) == null)
            {
                // create a new standard ApplicationUser account
                var user_User = new User()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_User,
                    Email = email_User
                };

                // insert the standard user into the DB
                await _userManager.CreateAsync(user_User, "MySecr3t$");

                // assign the "RegisteredUser" role
                await _userManager.AddToRoleAsync(user_User, role_RegisteredUser);

                // confirm the e-mail and remove lockout
                user_User.EmailConfirmed = true;
                user_User.LockoutEnabled = false;

                // add the standard user to the added users list
                addedUserList.Add(user_User);
            }

            // if we added at least one user, persist the changes into the DB
            if (addedUserList.Count > 0)
                await _context.SaveChangesAsync();
            return new JsonResult(new
            {
                Count = addedUserList.Count,
                Users = addedUserList
            });
        }
    }
}