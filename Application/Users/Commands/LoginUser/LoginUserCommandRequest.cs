using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Commands.LoginUser
{
    public sealed record LoginUserCommandRequest(string Email, string Password);
}
