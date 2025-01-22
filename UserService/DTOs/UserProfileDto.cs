namespace userService.DTOs
{
    public class UserProfileDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Address { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
