﻿using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ASPA.DAL.Security
{
    public class AdditionalUserClaimsPrincipalFactory
        : UserClaimsPrincipalFactory<User, Role>
    {
        public AdditionalUserClaimsPrincipalFactory(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
        { }

		public async override Task<ClaimsPrincipal> CreateAsync(User user)
		{
			var principal = await base.CreateAsync(user);
			var identity = (ClaimsIdentity)principal.Identity;

			var claims = new List<Claim>();
			if (user.IsAdmin)
			{
				claims.Add(new Claim(JwtClaimTypes.Role, "admin"));
			}
			else
			{
				claims.Add(new Claim(JwtClaimTypes.Role, "user"));
			}

			identity.AddClaims(claims);
			return principal;
		}
	}
}
