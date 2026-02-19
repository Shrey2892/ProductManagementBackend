namespace ProductManagementBackend.DTOs
{
    public class UpdateUserDto
    {
        public string? Username { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}
