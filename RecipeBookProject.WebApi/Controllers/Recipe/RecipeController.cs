using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeBookProject.Business.Abstract;
using RecipeBookProject.Business.Models;
using RecipeBookProject.Contracts;
using RecipeBookProject.Contracts.Recipes;
using RecipeBookProject.Data.Entities;
using RecipeBookProject.WebApi.Services;
using System.Security.Claims;

namespace RecipeBookProject.WebApi.Controllers.Recipe
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly IIngredientService _ingredientService;
        private readonly IFileService _fileService;

        public RecipeController(IRecipeService recipeService, IIngredientService ingredientService, IFileService fileService)
        {
            _recipeService = recipeService;
            _ingredientService = ingredientService;
            _fileService = fileService;
        }

        [HttpGet("getShortRecipes")]
        public async Task<IActionResult> GetRecipes()
        {
            var recipes = await _recipeService.GetShortRecipesAsync();
            return Ok(recipes);
        }

        [HttpGet("GetDetailedRecipe/{productId:int}")]
        public async Task<IActionResult> GetRecipes([FromRoute] int productId)
        {
            var recipe = await _recipeService.GetDetailedRecipeAsync(productId);
            return Ok(recipe);
        }

        [HttpGet("GetRecipeWithIngredients/{productId:int}")]
        public async Task<IActionResult> GetRecipeWithIngredients([FromRoute] int productId)
        {
            var recipe = await _recipeService.GetRecipeWithIngredientsAsync(productId);
            return Ok(recipe);
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var recipe = await _recipeService.GetAllCategoriesAsync();
            return Ok(recipe);
        }

        [HttpGet("GetAllIngredients")]
        public async Task<IActionResult> GetAllIngredients()
        {
            var ingredients = await _ingredientService.GetAllAsync();
            return Ok(GeneralResponse<List<IngredientDto>>.Success(ingredients, "Malzemeler başarıyla getirildi."));
        }

        [HttpPost("CreateIngredient")]
        public async Task<IActionResult> CreateIngredient([FromBody] IngredientDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(GeneralResponse<NoData>.Fail($"Validation hatası: {string.Join(", ", errors)}"));
            }

            var result = await _ingredientService.CreateAsync(dto);
            if (result)
            {
                return Ok(GeneralResponse<NoData>.Success("Malzeme başarıyla oluşturuldu."));
            }
            return BadRequest(GeneralResponse<NoData>.Fail("Malzeme oluşturulamadı."));
        }

        [HttpPut("UpdateIngredient/{id:int}")]
        public async Task<IActionResult> UpdateIngredient(int id, [FromBody] IngredientDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(GeneralResponse<NoData>.Fail($"Validation hatası: {string.Join(", ", errors)}"));
            }

            var result = await _ingredientService.UpdateAsync(id, dto);
            if (result)
            {
                return Ok(GeneralResponse<NoData>.Success("Malzeme başarıyla güncellendi."));
            }
            return BadRequest(GeneralResponse<NoData>.Fail("Malzeme güncellenemedi."));
        }

        [HttpDelete("DeleteIngredient/{id:int}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var result = await _ingredientService.DeleteAsync(id);
            if (result)
            {
                return Ok(GeneralResponse<NoData>.Success("Malzeme başarıyla silindi."));
            }
            return BadRequest(GeneralResponse<NoData>.Fail("Malzeme silinemedi."));
        }

        [HttpGet("TestStaticFile")]
        public IActionResult TestStaticFile()
        {
            try
            {
                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsPath = Path.Combine(webRootPath, "uploads");
                var imagesPath = Path.Combine(uploadsPath, "images");
                
                Console.WriteLine($"DEBUG: TestStaticFile - webRootPath: {webRootPath}");
                Console.WriteLine($"DEBUG: TestStaticFile - uploadsPath: {uploadsPath}");
                Console.WriteLine($"DEBUG: TestStaticFile - imagesPath: {imagesPath}");
                
                var uploadsExists = Directory.Exists(uploadsPath);
                var imagesExists = Directory.Exists(imagesPath);
                
                Console.WriteLine($"DEBUG: TestStaticFile - uploadsExists: {uploadsExists}");
                Console.WriteLine($"DEBUG: TestStaticFile - imagesExists: {imagesExists}");
                
                if (uploadsExists && imagesExists)
                {
                    var files = Directory.GetFiles(imagesPath);
                    Console.WriteLine($"DEBUG: TestStaticFile - Found {files.Length} files in images folder");
                    
                    // Test için bir dosya seç
                    var testFile = files.FirstOrDefault();
                    var testUrl = testFile != null ? $"/uploads/images/{Path.GetFileName(testFile)}" : "No file available";
                    
                    return Ok(new { 
                        webRootPath = webRootPath,
                        uploadsPath = uploadsPath,
                        imagesPath = imagesPath,
                        uploadsExists = uploadsExists,
                        imagesExists = imagesExists,
                        fileCount = files.Length,
                        sampleFiles = files.Take(5).Select(f => Path.GetFileName(f)).ToArray(),
                        testFileUrl = testUrl,
                        testFileExists = testFile != null
                    });
                }
                else
                {
                    return NotFound(new { 
                        webRootPath = webRootPath,
                        uploadsPath = uploadsPath,
                        imagesPath = imagesPath,
                        uploadsExists = uploadsExists,
                        imagesExists = imagesExists
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: TestStaticFile - Exception: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("TestImage/{fileName}")]
        public IActionResult TestImage(string fileName)
        {
            try
            {
                var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var imagePath = Path.Combine(webRootPath, "uploads", "images", fileName);
                
                Console.WriteLine($"DEBUG: TestImage - fileName: {fileName}");
                Console.WriteLine($"DEBUG: TestImage - webRootPath: {webRootPath}");
                Console.WriteLine($"DEBUG: TestImage - imagePath: {imagePath}");
                Console.WriteLine($"DEBUG: TestImage - File exists: {System.IO.File.Exists(imagePath)}");
                
                // FileService ile de test et
                var canAccessViaFileService = _fileService.TestImageAccess(fileName);
                Console.WriteLine($"DEBUG: TestImage - FileService access test: {canAccessViaFileService}");
                
                // Static file serving test
                var staticFileUrl = $"/uploads/images/{fileName}";
                Console.WriteLine($"DEBUG: TestImage - Static file URL: {staticFileUrl}");
                
                if (System.IO.File.Exists(imagePath))
                {
                    var fileInfo = new FileInfo(imagePath);
                    Console.WriteLine($"DEBUG: TestImage - File size: {fileInfo.Length} bytes");
                    return Ok(new { 
                        exists = true, 
                        path = imagePath, 
                        size = fileInfo.Length,
                        webRootPath = webRootPath,
                        uploadsPath = Path.Combine(webRootPath, "uploads"),
                        imagesPath = Path.Combine(webRootPath, "uploads", "images"),
                        staticFileUrl = staticFileUrl,
                        fileServiceAccess = canAccessViaFileService
                    });
                }
                else
                {
                    return NotFound(new { 
                        exists = false, 
                        path = imagePath, 
                        webRootPath = webRootPath,
                        uploadsPath = Path.Combine(webRootPath, "uploads"),
                        imagesPath = Path.Combine(webRootPath, "uploads", "images"),
                        staticFileUrl = staticFileUrl,
                        fileServiceAccess = canAccessViaFileService
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: TestImage - Exception: {ex.Message}");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                Console.WriteLine($"DEBUG: UploadImage called with file: {file?.FileName}, Size: {file?.Length} bytes");
                Console.WriteLine($"DEBUG: UploadImage - File content type: {file?.ContentType}");
                Console.WriteLine($"DEBUG: UploadImage - File extension: {Path.GetExtension(file?.FileName)}");
                
                if (file == null)
                {
                    Console.WriteLine("ERROR: File is null");
                    return BadRequest(GeneralResponse<NoData>.Fail("Dosya bulunamadı."));
                }
                
                var imageUrl = await _fileService.SaveImageAsync(file);
                Console.WriteLine($"DEBUG: File saved successfully, URL: {imageUrl}");
                Console.WriteLine($"DEBUG: UploadImage - URL length: {imageUrl?.Length}");
                Console.WriteLine($"DEBUG: UploadImage - URL contains extension: {imageUrl?.Contains(".")}");
                
                // Dosya erişimini test et
                var fileName = Path.GetFileName(imageUrl);
                var canAccess = _fileService.TestImageAccess(fileName);
                Console.WriteLine($"DEBUG: File access test result: {canAccess}");
                Console.WriteLine($"DEBUG: UploadImage - Extracted filename: {fileName}");
                
                return Ok(GeneralResponse<string>.Success(imageUrl, "Resim başarıyla yüklendi."));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ERROR: ArgumentException in UploadImage: {ex.Message}");
                return BadRequest(GeneralResponse<NoData>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Exception in UploadImage: {ex.Message}");
                Console.WriteLine($"ERROR: Stack trace: {ex.StackTrace}");
                return StatusCode(500, GeneralResponse<NoData>.Fail("Resim yüklenirken bir hata oluştu.", 500));
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetSearchedProducts([FromQuery] string query, [FromQuery] int? category)
        {
            var response = await _recipeService.GetSearchedProductsAsync(query, category);
            return Ok(response);
        }

        [HttpGet("{id:int}/comments")]
        public async Task<ActionResult<GeneralResponse<PagedResult<CommentDto>>>> GetComments(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sort = "new",
            CancellationToken ct = default)
        {
            if (User?.Identity?.IsAuthenticated != true)
                return Unauthorized(GeneralResponse<PagedResult<CommentDto>>.Fail("Oturum doğrulanamadı.", 401));

            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idStr, out var userId))
                return Unauthorized(GeneralResponse<PagedResult<CommentDto>>.Fail("Kullanıcı kimliği bulunamadı.", 401));

            var response = await _recipeService.GetProductCommentsAsync(id, userId, page, pageSize, sort, ct);

            return Ok(response);
        }

        [HttpPost("{id:int}/comments")]
        public async Task<ActionResult<GeneralResponse<PagedResult<CommentDto>>>> PostComments(int id, [FromBody] AddCommentDto dto, CancellationToken ct = default)
        {
            if (User?.Identity?.IsAuthenticated != true)
                return Unauthorized(GeneralResponse<PagedResult<CommentDto>>.Fail("Oturum doğrulanamadı.", 401));

            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idStr, out var userId))
                return Unauthorized(GeneralResponse<PagedResult<CommentDto>>.Fail("Kullanıcı kimliği bulunamadı.", 401));

            var response = await _recipeService.AddCommentsAsync(id, userId, dto, ct);

            return Ok(response);
        }

        [HttpPost("save-recipe")]
        public async Task<ActionResult<GeneralResponse<NoData>>> SaveRecipe([FromBody] SaveRecipeRequestDto request, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(GeneralResponse<NoData>.Fail("Kullanıcı doğrulanamadı.", 401));

            var result = await _recipeService.SaveRecipeAsync(userId, request.ProductId, request.IsSaved, ct);
            return Ok(result);
        }

        [HttpGet("getsavedrecipe")]
        public async Task<ActionResult<GeneralResponse<bool>>> GetSaveRecipe([FromQuery] int recipeId, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(GeneralResponse<bool>.Fail("Kullanıcı doğrulanamadı.", 401));

            var result = await _recipeService.GetSavedRecipeAsync(userId, recipeId, ct);
            return Ok(result);
        }

        [HttpPost("vote-recipe")]
        public async Task<ActionResult<GeneralResponse<NoData>>> VoteRecipe([FromBody] VoteRecipeDto request, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(GeneralResponse<NoData>.Fail("Kullanıcı doğrulanamadı.", 401));

            var result = await _recipeService.VoteRecipeAsync(userId, request.productid, request.vote, ct);
            return Ok(result);
        }

        [HttpGet("getvotedrecipe")]
        public async Task<ActionResult<GeneralResponse<VoteRecipeDto>>> GetVotedRecipe([FromQuery] int recipeId, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(GeneralResponse<VoteRecipeDto>.Fail("Kullanıcı doğrulanamadı.", 401));

            var result = await _recipeService.GetVotedRecipeAsync(userId, recipeId, ct);
            return Ok(result);
        }

        [HttpGet("getabusecategory")]
        public async Task<ActionResult<GeneralResponse<VoteRecipeDto>>> GetAbuseCategory(CancellationToken ct)
        {
            var result = await _recipeService.GetAbuseCategory();
            return Ok(result);
        }

        [HttpPost("saveabuse")]
        public async Task<ActionResult<GeneralResponse<NoData>>> SaveAbuse([FromBody] AbuseRequestDto request, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(GeneralResponse<NoData>.Fail("Kullanıcı doğrulanamadı.", 401));
            var result = await _recipeService.SaveAbuseAsync(userId, request);
            return Ok(result);
        }

        [HttpPost("createrecipe")]
        public async Task<ActionResult<GeneralResponse<NoData>>> CreateRecipe([FromBody] CreateProductDto request, CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(GeneralResponse<NoData>.Fail("Kullanıcı doğrulanamadı.", 401));
            var result = await _recipeService.CreateRecipeAsync(userId, request);
            return Ok(result);
        }
    }
}
