using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Abstractions.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Authentication;

public sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string Generate(Guid userId, string email)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email.ToString()),
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        string tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return tokenValue;
    }
}

#region Infrastructure => Application Baglantisi Hakkinda
/*
 * ? JwtProvider'da IJwtProvider Application katman�nda ve JwtProvider'da Infrastructure katman�nda. K�sacas� clean Architecture'da Infrastructure katman�ndan Application katman�na erismek dogru bir yaklasim midir ?
 * K�saca evet do�ru, bu soruyu aciklayacak olursak eger ;
 * Ic katmanlar (Application Domain), dis katmanlardan (Infrastructure ve Presentation) haberdar kesinlikle olmamalidir.
 * Bagimliliklar her zaman dis katmanlardan ic katmanlara olmalidir.
 * Bu yaklasim neden uygulanir ? Cunku ; Application katmani sadece interface'i tanimlar. Implementasyonun nasil yapilacagini Ic katman belirler. Bu durumda dis katman ic katmana bagimli olur.
 * Eger tam tersi olsaydi ; IJwtProvider interface'i Infrastructure da olsaydi Application katmani Infrastructure'a bagimli olurdu. Bu da Dependency Rule'a aykiridir. Ve istemedigimiz durumdur.
 */
#endregion