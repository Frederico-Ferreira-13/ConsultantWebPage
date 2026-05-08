using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    public class AboutMe : IEntity
    {
        [Key]
        public int AboutMeId { get; protected set; }
        [Required]
        public int UserId { get; protected set; }
        [MaxLength(2000)]
        public string Bio { get; protected set; } = string.Empty;
        [MaxLength(500)]
        public string ProfileImageUrl { get; protected set; } = string.Empty;
        public int YearsOfExperience { get; protected set; }
        [MaxLength(500)]
        public string SocialUrl { get; protected set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public virtual Users? User { get; private set; }

        private AboutMe() {}

        public AboutMe(int userId, string bio, string profilePictureUrl, int yearsOfExperience, string socialUrl)
        {
            ValidateAboutMeData(userId, bio, profilePictureUrl, yearsOfExperience, socialUrl);


            UserId = userId;
            Bio = bio;
            ProfileImageUrl = profilePictureUrl;
            YearsOfExperience = yearsOfExperience;
            SocialUrl = socialUrl;
        }

        public void UpdateAboutMe(string bio, string profileImageUrl, int yearsOfExperience, string socialUrl)
        {
            ValidateAboutMeData(UserId, bio, profileImageUrl, yearsOfExperience, socialUrl);

            Bio = bio;
            ProfileImageUrl = profileImageUrl;
            YearsOfExperience = yearsOfExperience;
            SocialUrl = socialUrl;
        }

        public int GetId() => AboutMeId;

        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be a positive integer.", nameof(id));

            AboutMeId = id;
        }

        public bool GetIsActive() => true; // Assuming AboutMe is always active

        private void ValidateAboutMeData(int userId, string bio, string profileImageUrl, int yearsOfExperience, string socialUrl)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be a positive integer.", nameof(userId));
            if (string.IsNullOrWhiteSpace(bio))
                throw new ArgumentException("Bio cannot be empty.", nameof(bio));
            if (string.IsNullOrWhiteSpace(profileImageUrl))
                throw new ArgumentException("Profile image URL cannot be empty.", nameof(profileImageUrl));
            if (yearsOfExperience < 0)
                throw new ArgumentException("Years of experience cannot be negative.", nameof(yearsOfExperience));
            if (string.IsNullOrWhiteSpace(socialUrl))
                throw new ArgumentException("Social URL cannot be empty.", nameof(socialUrl));
        }

        
    }
}


