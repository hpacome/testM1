using ASPA.BOL.Dto;
using ASPA.BOL.OutputModel.ApplicationUser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ASPA.BLL.Service.ApplicationUser
{
    public interface IApplicationUserService
    {
        Task<RegistrationOutputModel> RegisterApplicationUser(ApplicationUserDto userDto);

        Task<LoginOutputModel> LoginApplicationUser(string email, string password);

        Task<RegistrationOutputModel> SendConfirmationEmail(string email);

        Task<RegistrationOutputModel> ConfirmationEmail(string userId, string code);
    }
}
