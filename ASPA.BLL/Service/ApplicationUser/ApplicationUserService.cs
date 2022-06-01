using ASPA.BOL;
using ASPA.BOL.Dto;
using ASPA.BOL.Mapping;
using ASPA.BOL.OutputModel;
using ASPA.BOL.OutputModel.ApplicationUser;
using ASPA.DAL.Repository.ApplicationUser;
using ASPA.DAL.Security;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ASPA.BLL.Service.ApplicationUser
{
    public class ApplicationUserService : IApplicationUserService
    {
        private IApplicationUserRepository _applicationUserRepository;
        private UserManager<User> _userManager;
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment _host;

        public ApplicationUserService(
            IApplicationUserRepository applicationUserRepository, 
            UserManager<User> userManager, 
            IOptions<AppSettings> appSettings,
            IHostingEnvironment host)
        {
            _applicationUserRepository = applicationUserRepository;
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _host = host;
        }

        /// <summary>
        /// Register new User
        /// </summary>
        /// <param name="userDto">New user informations</param>
        public async Task<RegistrationOutputModel> RegisterApplicationUser(ApplicationUserDto userDto)
        {
            var user = ApplicationUserMapping.toEntity(userDto);
            try
            {
                var result = await _userManager.CreateAsync(user, userDto.Password);

                if (userDto.RoleString != null)
                    await _userManager.AddToRolesAsync(user, userDto.RoleString);

                if (result.Succeeded)
                {
                    var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationUrl = $@"{_appSettings.Client_URL}/applicationUser/confirm-email?userid={user.Id}&code={this.Base64UrlEncode(confirmationToken)}";

                    await this.SendConfirmEmailLink(user, confirmationUrl);
                }

                return new RegistrationOutputModel
                {
                    Succeded = result.Succeeded,
                    RegistrationEmail = user.Email,
                    Info = result.Errors.Select(x => new OutputMessage
                    {
                        Code = x.Code,
                        Message = x.Description
                    })
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// sent confirmation email adress
        /// </summary>
        /// <param name="email">user email</param>
        public async Task<RegistrationOutputModel> SendConfirmationEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                var message = new OutputMessage
                {
                    Code = "UserNoteFound",
                    Message = $@"the user with email '{email}' does not exist"
                };
                var infos = new List<OutputMessage>();
                infos.Add(message);
                return new RegistrationOutputModel
                {
                    Succeded = false,
                    Info = infos
                };
            }
            else
            {
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationUrl = $@"{_appSettings.Client_URL}/applicationUser/confirm-email?userid={user.Id}&code={this.Base64UrlEncode(confirmationToken)}";

                await this.SendConfirmEmailLink(user, confirmationUrl);

                return new RegistrationOutputModel
                {
                    Succeded = true,
                    RegistrationEmail = user.Email
                };
            }
        }

        /// <summary>
        /// Confirm user mail
        /// </summary>
        /// <param name="userId">user id and confirmation code</param>
        ///  <param name="code">confirmation code</param>
        public async Task<RegistrationOutputModel> ConfirmationEmail(string userId, string code)
        {
            User user = await _userManager.FindByIdAsync(userId);

            if(user == null)
            {
                var message = new OutputMessage
                {
                    Code = "UserNoteFound",
                    Message = $@"confirmation failed : email '{user.Email}'"
                };
                var infos = new List<OutputMessage>();
                infos.Add(message);
                return new RegistrationOutputModel
                {
                    Succeded = false,
                    RegistrationEmail = user.Email,
                    Info = infos
                };

            }
            else
            {
                var token = this.Base64UrlDecode(code);
                IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);

                return new RegistrationOutputModel
                {
                    Succeded = result.Succeeded,
                    RegistrationEmail = result.Succeeded ? user.Email : null,
                    Info = result.Errors.Select(x => new OutputMessage
                    {
                        Code = x.Code,
                        Message = x.Description
                    })
                };
            }      
        }


        /// <summary>
        /// Login application user
        /// </summary>
        /// <param name="email">user email</param>
        /// <param name="password">user password</param>
        public async Task<LoginOutputModel> LoginApplicationUser(string email, string password)
        {
            var applicationUser = await _userManager.FindByEmailAsync(email);

            if (applicationUser == null)
            {
                var message = new OutputMessage
                {
                    Code = "UserNoteFound",
                    Message = $@"the user with email '{email}' does not exist"
                };
                var infos = new List<OutputMessage>();
                infos.Add(message);
                return new LoginOutputModel
                {
                    Succeded = false,
                    Info = infos
                };
            }
            else if (!applicationUser.EmailConfirmed)
            {
                var message = new OutputMessage
                {
                    Code = "EmailNOtConfirmed",
                    Message = $@"the email '{email}' is not yet confirmed"
                };
                var infos = new List<OutputMessage>();
                infos.Add(message);
                return new LoginOutputModel
                {
                    Succeded = false,
                    Info = infos
                };
            }
            else if (!applicationUser.LockoutEnabled)
            {
                var message = new OutputMessage
                {
                    Code = "AccountLockout",
                    Message = $@"the account with the email '{email}' is locked, please contact the administrator"
                };
                var infos = new List<OutputMessage>();
                infos.Add(message);
                return new LoginOutputModel
                {
                    Succeded = false,
                    Info = infos
                };
            }
            else
            {
                if (await _userManager.CheckPasswordAsync(applicationUser, password))
                {
                    var key = Encoding.UTF8.GetBytes(_appSettings.JWT_Secret);

                    List<Claim> identity = new List<Claim>();
                    identity.Add(new Claim("UserID", applicationUser.Id.ToString()));

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(identity),
                        Expires = DateTime.UtcNow.AddMinutes(_appSettings.Token_expiration_minutes),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);

                    return new LoginOutputModel
                    {
                        Succeded = true,
                        Id = applicationUser.Id,
                        Token = token,
                    };
                }
                else
                {
                    var message = new OutputMessage
                    {
                        Code = "IncorrectPassword",
                        Message = $@"The password you’ve entered is incorrect."
                    };
                    var infos = new List<OutputMessage>();
                    infos.Add(message);
                    return new LoginOutputModel
                    {
                        Succeded = false,
                        Info = infos
                    };
                }
            }
        }

        #region private function

        /// <summary>
        /// Decode Url code
        /// </summary>
        /// <param name="code">string to decode</param>
        private string Base64UrlDecode(string code)
        {
            var codeByte = WebEncoders.Base64UrlDecode(code);
            return Encoding.UTF8.GetString(codeByte);
        }

        /// <summary>
        /// Encode token
        /// </summary>
        /// <param name="token">token to encode</param>
        private string Base64UrlEncode(string token)
        {
            return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        }

        /// <summary>
        /// Send Confirmation email link
        /// </summary>
        /// <param name="user">user</param>
        /// <param name="link">confirmation link</param>
        private async Task<bool> SendConfirmEmailLink(User user, string link)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(_appSettings.Application_email_adresse);
                msg.To.Add(user.Email);

                string templatePath = $@"{_host.ContentRootPath}/Pages/EmaiTemplate/ConfirmEmail.html";

                using (var reader = new StreamReader(templatePath))
                {
                    int linkExpirationPeriode = Convert.ToInt32(_appSettings.Token_expiration_minutes)/60;
                    string readFile = reader.ReadToEnd();
                    string strContent = readFile;
                    strContent = strContent.Replace("[Name]", user.FirstName);
                    strContent = strContent.Replace("[linkExpirationPeriode]", linkExpirationPeriode.ToString());
                    strContent = strContent.Replace("[link]", link);
                    msg.Subject = "Account confirmation";
                    msg.Body = strContent;
                    msg.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = _appSettings.Mail_Smtp_Host;
                    System.Net.NetworkCredential credential = new System.Net.NetworkCredential();
                    credential.UserName = _appSettings.Application_email_adresse;
                    credential.Password = _appSettings.Application_email_password;
                    smtp.Credentials = credential;
                    smtp.Port = Convert.ToInt32(_appSettings.Application_email_port);
                    smtp.EnableSsl = true;
                    smtp.SendAsync(msg, null);
                }
                return true;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}
