using Bangk.Common;
using Bangk.Common.Dto;
using Bangk.Domain.Contract.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : BaseController
    {
        private readonly IUserLoginService _userLoginService;
        private readonly IResetPasswordService _resetPasswordService;
        private readonly ISendEmailResetPassword _sendEmailResetPassword;

        public AuthenticationController(IUserLoginService userLoginService, IResetPasswordService resetPasswordService, ISendEmailResetPassword sendEmailResetPassword)
        {
            _userLoginService = userLoginService;
            _resetPasswordService = resetPasswordService;
            _sendEmailResetPassword = sendEmailResetPassword;
        }

        /// <summary>
        /// User Login
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var result = await _userLoginService.Execute(login);
            if (result.HasErrors)
            {
                return BadRequest(result.Errors);
            }

            Logs.WriteInfoLog($"Login successful. User: [{login.Email}].");
            return Ok(new
            {
                token = result.Result
            });
        }


        /// <summary>
        /// Send Email Reset Password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("SendEmailResetPassword")]
        public async Task<IActionResult> SendEmailResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("El correo es obligatorio");
            }

            var result = await _sendEmailResetPassword.Execute(email);
            if (result.HasErrors)
            {
                return BadRequest(result.Errors);
            }

            Logs.WriteInfoLog($"Send Email Reset password successful. Email: [{email}].");
            return Ok();
        }
        
        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="resetPasswordDto"></param>
        /// <returns></returns>
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var result = await _resetPasswordService.Execute(resetPasswordDto);
            if (result.HasErrors)
            {
                return BadRequest(result.Errors);
            }

            Logs.WriteInfoLog($"Reset password successful.");
            return Ok();
        }
    }
}
