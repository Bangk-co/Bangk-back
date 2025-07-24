using Bangk.Common;
using Bangk.Common.Dto;
using Bangk.Domain.Contract.Login;
using Bangk.Domain.Contract.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        private readonly ISendCodeValidationEmailService _sendCodeValidationEmailService;
        private readonly IReSendCodeValidationEmailService _reSendCodeValidationEmailService;
        private readonly IUpdatePasswordService _updatePasswordService;
        private readonly ISendCodeOtpEmailService _sendCodeOtpEmailService;
        public AuthenticationController(IUserLoginService userLoginService, IResetPasswordService resetPasswordService, ISendEmailResetPassword sendEmailResetPassword,
            ISendCodeValidationEmailService sendCodeValidationEmailService, IReSendCodeValidationEmailService reSendCodeValidationEmailService, IUpdatePasswordService updatePasswordService
            ,ISendCodeOtpEmailService sendCodeOtpEmailService)
        {
            _userLoginService = userLoginService;
            _resetPasswordService = resetPasswordService;
            _sendEmailResetPassword = sendEmailResetPassword;
            _sendCodeValidationEmailService = sendCodeValidationEmailService;
            _reSendCodeValidationEmailService = reSendCodeValidationEmailService;
            _updatePasswordService = updatePasswordService;
            _sendCodeOtpEmailService = sendCodeOtpEmailService;
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
            var result = await _sendEmailResetPassword.Execute(email);
            if (result.HasErrors)
            {
                return BadRequest(result.Errors);
            }

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

            return Ok();
        }

        /// <summary>
        /// Send Email Reset Password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("SendCodeValidationEmail")]
        public async Task<IActionResult> SendCodeValidationEmail(SendCodeValidationEmailDto sendCodeValidationEmailDto)
        {
            var result = await _sendCodeValidationEmailService.Execute(sendCodeValidationEmailDto);
            if (result.HasErrors)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new
            {
                code = result.Result
            });
        }

        /// <summary>
        /// Send Email Reset Password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("SendCodeOtpEmail")]
        public async Task<IActionResult> SendCodeOtpEmail(SendCodeValidationEmailDto sendCodeValidationEmailDto)
        {
            var result = await _sendCodeOtpEmailService.Execute(sendCodeValidationEmailDto);
            if (result.HasErrors)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new
            {
                code = result.Result
            });
        }

        /// <summary>
        /// Send Email Reset Password
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("ReSendCodeValidationEmail")]
        public async Task<IActionResult> ReSendCodeValidationEmail(string email)
        {
            var result = await _reSendCodeValidationEmailService.Execute(email);
            if (result.HasErrors)
            {
                return BadRequest(result.Errors);
            }

            return Ok(new
            {
                code = result.Result
            });
        }

        [HttpPut("UpdatePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordDto updatePasswordDto)
        {
            var result = await _updatePasswordService.Execute(updatePasswordDto);
            if (result.HasErrors)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }
    }
}
