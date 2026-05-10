using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models.ValueObjects;

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
        
        public Money Price { get; protected set; } = null!;

        public Address Address { get; protected set; } = null!;

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

        private RealEstate(int realEstateTypeId, int consultantId, string title, string thumbnailUrl,
            string externalSourceUrl, string externalReferenceId, Money price, Address address, string status, bool isFeatured)
        {
            RealEstateTypeId = realEstateTypeId;
            ConsultantId = consultantId;
            Title = title;
            ThumbnailUrl = thumbnailUrl;
            ExternalSourceUrl = externalSourceUrl;
            ExternalReferenceId = externalReferenceId;
            Price = price;
            Address = address;
            Status = status;
            IsFeatured = isFeatured;
            CreatedAt = DateTime.UtcNow;
            LastCheckDate = DateTime.UtcNow;
        }

        public static Result<RealEstate> Create(int realEstateTypeId, int consultantId, string title, string thumbnailUrl,
            string externalSourceUrl, string externalReferenceId, Money price, Address address, string status, bool isFeatured)
        {
            if (realEstateTypeId <= 0) return Error.Validation("Tipo de imóvel inválido.");
            if (consultantId <= 0) return Error.Validation("Consultor inválido.");
            if (string.IsNullOrWhiteSpace(title)) return Error.Validation("O título é obrigatório.");
            if (price == null || price.Amount < 0) return Error.Validation("O preço é inválido.");
            if (address == null) return Error.Validation("A morada é obrigatória.");

            return new RealEstate(realEstateTypeId, consultantId, title, thumbnailUrl, 
                                 externalSourceUrl, externalReferenceId, price, address, status, isFeatured);
        }

        public void UpdateRealEstate(int realEstateTypeId, int consultantId, string title, string thumbnailUrl,
            string externalSourceUrl, string externalReferenceId, Money price, Address address, string status, bool isFeatured)
        {            
            if (price != null) Price = price;
            
            RealEstateTypeId = realEstateTypeId;
            ConsultantId = consultantId;
            Title = title;
            ThumbnailUrl = thumbnailUrl;
            ExternalSourceUrl = externalSourceUrl;
            ExternalReferenceId = externalReferenceId;
            Address = address;
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
    }
}
