using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBookProject.Business.Models;
using RecipeBookProject.Contracts;
using RecipeBookProject.Contracts;

namespace RecipeBookProject.Business.Abstract
{
    public interface IProfileService
    {
        Task<GeneralResponse<List<ProductDto>>> GetUserProfileAsync(int? userid);
        Task<GeneralResponse<UserDetails>> GetUserDetailAsync(int? userid);
        Task<GeneralResponse<NoData>> UpdateProfileDetailsAsync(UserDetails detail,int? id);
    }
}
