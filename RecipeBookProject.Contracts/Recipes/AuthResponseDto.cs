using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Models
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
        public AuthResponseDto(string a)
        {
            AccessToken = a;
        }
    }
}
