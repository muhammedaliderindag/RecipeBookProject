using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Business.Models;
using RecipeBookProject.Data.Entities;
using RecipeBookProject.DataAccess.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBookProject.Business.Concrete
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;

        public ProfileService(IProfileRepository profileRepository, IUserRepository userRepository)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
        }

        public async Task<GeneralResponse<UserDetails>> GetUserDetailAsync(int? userid)
        {
            if (userid == null)
            {
                throw new Exception("Kullanıcı ID'si boş olamaz");
            }

            var response = await _profileRepository.GetUserDetailRepositoryAsync(userid);

            if (response is null)
            {
                return GeneralResponse<UserDetails>.Fail("Response bulunamadı.", 404);
            }

            var userDetails = new UserDetails
            {
                ProfileImageUrl = response.ProfileImageUrl,
                Bio = response.Bio,
                PersonalWebsite = response.PersonalWebsite
            };

            return GeneralResponse<UserDetails>.Success(userDetails,"Veriler başarıyla çekildi",200);
        }

        public async Task<GeneralResponse<List<ProductDto>>> GetUserProfileAsync(int? userid)
        {
            if (userid == null)
            {
                throw new Exception("Kullanıcı ID'si boş olamaz");
            }
            var products = await _profileRepository.GetSavedProductRepositoryAsync(userid);
            if (products == null || !products.Any())
            {
                throw new Exception("Kullanıcı profili veya ürünler bulunamadı");
            }
            var profileDto = products.Select(p => new ProductDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    ProductShortDesc = p.ProductShortDesc,
                    ImageUrl = p.ImageUrl,
                    CategoryId = p.CategoryId,
                    Category = p.Category == null ? null : new CategoryDto
                    {
                        CategoryId = p.Category.CategoryId,
                        CategoryName = p.Category.CategoryName
                    },
                    FeaturedCategory = p.FeaturedCategory == null ? null : new FeaturedCategoryDto
                    {
                        FeaturedCategoryId = p.FeaturedCategory.FeaturedCategoryId,
                        FeaturedCategoryName = p.FeaturedCategory.FeaturedCategoryName
                    },
                }).ToList();

            return GeneralResponse<List<ProductDto>>.Success(profileDto, "Profil başarıyla çekildi",200);
        }

        public async Task<GeneralResponse<NoData>> UpdateProfileDetailsAsync(UserDetails detail, int? id)
        {
            if (detail is null || id is null)
                return GeneralResponse<NoData>.Fail("Geçersiz veri.", 400);

            var user = await _userRepository.GetUserByIdAsync(id); // NoTracking KULLANMAYIN

            if (user is null)
                return GeneralResponse<NoData>.Fail("Kullanıcı bulunamadı.", 404);

            // Kısmi güncelleme isteniyorsa ?? kullanın
            user.Bio = detail.Bio ?? user.Bio;
            user.PersonalWebsite = detail.PersonalWebsite ?? user.PersonalWebsite;
            user.ProfileImageUrl = detail.ProfileImageUrl ?? user.ProfileImageUrl;

            await _userRepository.UpdateAsync(user);        // AddAsync DEĞİL
                                                            // veya repository içinde SaveChangesAsync çağrılıyorsa Update yerine Save yeterli olabilir

            return GeneralResponse<NoData>.Success("Profil güncellendi.", 200); // veya 204
        }

    }
}
