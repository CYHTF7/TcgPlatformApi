using System.ComponentModel.DataAnnotations;

namespace TcgPlatformApi.Models
{
    public class PlayerProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(32)]
        public required string Nickname { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public required string Email { get; set; }

        public string ?EmailCode { get; set; }

        [Required]
        [MaxLength(256)]
        public string ?PasswordHash { get; set; }

        public string ?PasswordResetCode { get; set; }

        [MaxLength(512)]
        public string AvatarPath { get; set; } = "default_avatar.png";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsVerified { get; set; }

        //jwt
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        //statistics
        public int BattlesPlayed { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Gold { get; set; }
    }
}
