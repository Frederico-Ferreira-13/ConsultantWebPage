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
        
        public string Bio { get; protected set; } = string.Empty;        
        
        public string ProfileImageUrl { get; protected set; } = string.Empty;
        
        public int YearsOfExperience { get; protected set; }        
        
        public string SocialUrl_Linkedin { get; protected set; } = string.Empty;
        public string SocialUrl_X { get; protected set; } = string.Empty;
        public string SocialUrl_Facebook { get; protected set; } = string.Empty;
        public string SocialUrl_Instagram { get; protected set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public virtual Users? User { get; private set; }

        private AboutMe() {}

        public AboutMe(int userId, string bio, string profileImageUrl, int yearsOfExperience, string linkedIn, string instagram, string x, string facebook)
        {
            ValidateAboutMeData(userId, yearsOfExperience);

            UserId = userId;
            Bio = bio;
            ProfileImageUrl = profileImageUrl;
            YearsOfExperience = yearsOfExperience;
            SocialUrl_Linkedin = linkedIn;
            SocialUrl_Instagram = instagram;
            SocialUrl_X = x;
            SocialUrl_Facebook = facebook;
        }

        public void UpdateAboutMe(string bio, string profileImageUrl, int yearsOfExperience, string linkedIn, string instagram,
            string x, string facebook)
        {
            ValidateAboutMeData(UserId, yearsOfExperience);

            Bio = bio;
            ProfileImageUrl = profileImageUrl;
            YearsOfExperience = yearsOfExperience;
            SocialUrl_Linkedin = linkedIn;
            SocialUrl_Instagram = instagram;
            SocialUrl_X = x;
            SocialUrl_Facebook = facebook;
        }

        public int GetId() => AboutMeId;

        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be a positive integer.", nameof(id));

            AboutMeId = id;
        }

        public bool GetIsActive() => true; // Assuming AboutMe is always active

        private void ValidateAboutMeData(int userId, int yearsOfExperience)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be a positive integer.", nameof(userId));
            
            if (yearsOfExperience < 0)
                throw new ArgumentException("Years of experience cannot be negative.", nameof(yearsOfExperience));
        }        
    }
}


