using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Users.Commands.LoginUser;
using Application.Users.Commands.RegisterUser;
using MediatR;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ApiController
    {
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserCommandRequest request)
        {
            var command = new RegisterUserCommand(request.Email, request.Password);
            var userId = await Sender.Send(command);
            return Ok(new { UserId = userId, Message = "Kullanıcı başarıyla oluşturuldu." });
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginUserCommandRequest request)
        {
            var command = new LoginUserCommand(request.Email, request.Password);
            var token = await Sender.Send(command);
            return Ok(new { Token = token });
        }
    }
} 