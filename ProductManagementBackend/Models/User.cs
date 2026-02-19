namespace ProductManagementBackend.Models
{
  
        public class User
        {
            public int Id { get; set; }
            public string Username { get; set; } = "";
            public string Email { get; set; } = "";
            public string PasswordHash { get; set; } = "";

            public string Role { get; set; } = "User";
            public bool IsApproved { get; set; }
            public bool IsRestricted { get; set; }

            public bool IsEmailVerified { get; set; }
            public string? EmailOtp { get; set; }
            public DateTime? OtpExpiry { get; set; }

            public string? ImagePath { get; set; }

            public string? PasswordResetToken { get; set; }
            public DateTime? TokenExpiry { get; set; }


        }
    

}
