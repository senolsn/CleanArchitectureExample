using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Exceptions.Base;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands.LoginUser;

internal sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, string>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IJwtProvider _jwtProvider;

    public LoginUserCommandHandler(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new UserNotFoundException("Kullanıcı bulunamadı");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
        {
            throw new BusinessException("Kullanıcı adı veya şifreniz yanlış.");
        }

        var token = _jwtProvider.Generate(Guid.Parse(user.Id), user.Email);

        return token;
    }
} 