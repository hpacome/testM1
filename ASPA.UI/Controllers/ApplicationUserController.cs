using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPA.BLL.Service.ApplicationUser;
using ASPA.BOL.Dto;
using ASPA.BOL.InputModel.ApplicationUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASPA.UI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private readonly IApplicationUserService _applicationUserRepository;

        public ApplicationUserController(IApplicationUserService applicationUserRepository)
        {
            _applicationUserRepository = applicationUserRepository;
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegistrationInputModel inputModel)
        {
            var userDto = new ApplicationUserDto
            {
                FirstName = inputModel.FirstName,
                LastName = inputModel.LastName,
                UserName = inputModel.Email,
                Email = inputModel.Email,
                Password = inputModel.Password,
            };
            var registrationResult = await _applicationUserRepository.RegisterApplicationUser(userDto);
            return Ok(registrationResult);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel inputModel)
        {
            var loginResult = await _applicationUserRepository.LoginApplicationUser(inputModel.Email, inputModel.Password);
            return Ok(loginResult);
        }

        [HttpGet]
        public async Task<IActionResult> SendConfirmationEmail(string email)
        {
            var sendConfirmEmailResult = await _applicationUserRepository.SendConfirmationEmail(email);
            return Ok(sendConfirmEmailResult);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailInputModel inputModel)
        {
            var confirmEmailResult = await this._applicationUserRepository.ConfirmationEmail(inputModel.UserId, inputModel.ConfirmationCode);
            return Ok(confirmEmailResult);
        }

    }
}