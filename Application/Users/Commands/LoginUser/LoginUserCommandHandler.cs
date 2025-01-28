using System.Security.Cryptography;
using System.Text;
using Application.Abstractions.Authentication;
using Application.Abstractions.Messaging;
using Domain.Abstractions;

namespace Application.Users.Commands.LoginUser;

internal sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, string>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public LoginUserCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            throw new Exception("User with given email does not exist");
        }

        var passwordHash = HashPassword(request.Password);

        if (user.PasswordHash != passwordHash)
        {
            throw new Exception("Invalid password");
        }

        var token = _jwtProvider.Generate(user.Id, user.Email);

        return token;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
} 