namespace RecipeBookProject.Client.Models
{
    public class RegisterRequestDto
    {
        public string Email { get; set; } = String.Empty;
        public string PasswordHashed { get; set; } = String.Empty;
        public string FirstName { get; set; } = String.Empty;
        public string LastName { get; set; } = String.Empty;
    }
}
