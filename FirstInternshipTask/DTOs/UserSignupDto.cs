using System.ComponentModel.DataAnnotations;


namespace FirstInternshipTask.DTO
{
    public class UserSignupDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string Email { get; set; } = string.Empty.ToString();

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

    }
}
