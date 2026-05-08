using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    public class RealEstate : IEntity
    {
        [Key]
        public int RealEstateId { get; protected set; }
        public int RealEstateTypeId { get; protected set; }
        public int ConsultantId { get; protected set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; protected set; } = string.Empty;

        public string ThumbnailUrl { get; protected set; } = string.Empty;

        public string ExternalSourceUrl { get; protected set; } = string.Empty;

        [MaxLength(100)]
        public string ExternalReferenceId { get; protected set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; protected set; }

        [MaxLength(100)]
        public string City { get; protected set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; protected set; } = string.Empty;

        public bool IsFeatured { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime LastCheckDate { get; protected set; }

        [ForeignKey("RealEstateTypeId")]
        public virtual RealEstateType? RealEstateType { get; private set; }

        [ForeignKey("ConsultantId")]
        public virtual Users? Consultant { get; private set; }

        private RealEstate() { }

        public RealEstate(int realEstateTypeId, int consultantId, string title, string thumbnailUrl,
            string externalSourceUrl, string externalReferenceId, decimal price, string city, string status, bool isFeatured)
        {
            ValidateRealEstate(realEstateTypeId, consultantId, title, thumbnailUrl, price, city);

            RealEstateTypeId = realEstateTypeId;
            ConsultantId = consultantId;
            Title = title;
            ThumbnailUrl = thumbnailUrl;
            ExternalSourceUrl = externalSourceUrl;
            ExternalReferenceId = externalReferenceId;
            Price = price;
            City = city;
            Status = status;
            IsFeatured = isFeatured;
            CreatedAt = DateTime.UtcNow;
            LastCheckDate = DateTime.UtcNow;
        }

        public void UpdateRealEstate(int realEstateTypeId, int consultantId, string title, string thumbnailUrl,
            string externalSourceUrl, string externalReferenceId, decimal price, string city, string status, bool isFeatured)
        {
            ValidateRealEstate(realEstateTypeId, consultantId, title, thumbnailUrl, price, city);

            RealEstateTypeId = realEstateTypeId;
            ConsultantId = consultantId;
            Title = title;
            ThumbnailUrl = thumbnailUrl;
            ExternalSourceUrl = externalSourceUrl;
            ExternalReferenceId = externalReferenceId;
            Price = price;
            City = city;
            Status = status;
            IsFeatured = isFeatured;
            LastCheckDate = DateTime.UtcNow;
        }

        public int GetId() => RealEstateId;

        public void SetId(int id)
        {
            if (RealEstateId != 0)
                throw new InvalidOperationException("Não é permitido alterar o ID de um imóvel que já possui um.");

            if (id <= 0)
                throw new ArgumentException("O ID deve ser um inteiro positivo.");

            RealEstateId = id;
        }

        public bool GetIsActive() => Status == "Available";

        private void ValidateRealEstate(int realEstateTypeId, int consultantId, string title, string thumbnailUrl, decimal price, string city)
        {
            if (realEstateTypeId <= 0) throw new ArgumentException("Tipo de imóvel inválido.");
            if (consultantId <= 0) throw new ArgumentException("Consultor inválido.");
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("O título é obrigatório.");
            if (price < 0) throw new ArgumentException("O preço não pode ser negativo.");
        }
    }
}
