using ASPA.DAL.Repository.ApplicationUser;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASPA.BLL
{
    public static class ServicesDependency
    {
        public static void AddServiceDependency(this IServiceCollection services)
        {
            services.AddTransient<IApplicationUserRepository, ApplicationUserRepository>();
        }
    }
}
