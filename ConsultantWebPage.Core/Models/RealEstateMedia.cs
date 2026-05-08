using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    public class RealEstateMedia : IEntity
    {
        [Key]
        public int MediaId { get; protected set; }
        public int RealEstateId { get; protected set; }

        [Required]
        public string MediaUrl { get; protected set; } = string.Empty;

        [MaxLength(50)]
        public string MediaType { get; protected set; } = string.Empty;
        public bool IsMain { get; protected set; }

        [ForeignKey("RealEstateId")]
        public virtual RealEstate? RealEstate { get; private set; }

        private RealEstateMedia() { }

        public RealEstateMedia(int realEstateId, string mediaUrl, string mediaType, bool isMain)
        {
            ValidateRealEstateMedia(realEstateId, mediaUrl, mediaType);

            RealEstateId = realEstateId;
            MediaUrl = mediaUrl;
            MediaType = mediaType;
            IsMain = isMain;
        }

        private void ValidateRealEstateMedia(int realEstateId, string mediaUrl, string mediaType)
        {
            if (realEstateId <= 0)
                throw new ArgumentException("Real estate ID must be a positive integer.");

            if (string.IsNullOrWhiteSpace(mediaUrl))
                throw new ArgumentException("Media URL cannot be empty.");

            if (string.IsNullOrWhiteSpace(mediaType))
                throw new ArgumentException("Media type cannot be empty.");
        }

        public void UpdateRealEstateMedia(int realEstateId, string mediaUrl, string mediaType, bool isMain)
        {
            ValidateRealEstateMedia(realEstateId, mediaUrl, mediaType);

            RealEstateId = realEstateId;
            MediaUrl = mediaUrl;
            MediaType = mediaType;
            IsMain = isMain;
        }

        public int GetId() => MediaId;

        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be a positive integer.");

            MediaId = id;
        }

        public bool GetIsActive() => true; // Assuming RealEstateMedia is always active, adjust as needed
    }
}
