using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Models
{
    public class LoginRequestDto
    {
        public string Email { get; }
        public string Password { get; }

        public LoginRequestDto(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
