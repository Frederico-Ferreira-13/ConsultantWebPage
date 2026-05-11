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
        public int CityId { get; protected set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; protected set; } = string.Empty;
        public string? Description { get; protected set; }
        public string? ThumbnailUrl { get; protected set; }

        public string? ExternalSourceUrl { get; protected set; }

        [MaxLength(100)]
        public string? ExternalReferenceId { get; protected set; }
        
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
        
        [ForeignKey("CityId")]
        public virtual City? City { get; private set; }

        private RealEstate() { }

        private RealEstate(int realEstateTypeId, int consultantId, int cityId, string title, string? description, 
            string? thumbnailUrl, string? externalSourceUrl, string? externalReferenceId, 
            Money price, Address address, string status, bool isFeatured)
        {
            RealEstateTypeId = realEstateTypeId;
            ConsultantId = consultantId;
            CityId = cityId;
            Title = title;
            Description = description;
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

        public static Result<RealEstate> Create(int realEstateTypeId, int consultantId, int cityId, string title, 
            string? description, string? thumbnailUrl, string? externalSourceUrl, string? externalReferenceId, 
            Money price, Address address, string status, bool isFeatured)
        {
            if (realEstateTypeId <= 0) return Error.Validation("Tipo de imóvel inválido.");
            if (consultantId <= 0) return Error.Validation("Consultor inválido.");
            if (cityId <= 0) return Error.Validation("Cidade inválida.");
            if (string.IsNullOrWhiteSpace(title)) return Error.Validation("O título é obrigatório.");
            if (price == null || price.Amount < 0) return Error.Validation("O preço é inválido.");
            if (address == null) return Error.Validation("A morada é obrigatória.");

            return new RealEstate(realEstateTypeId, consultantId, cityId, title, description, 
                                 thumbnailUrl, externalSourceUrl, externalReferenceId, price, address, status, isFeatured);
        }

        public void UpdateRealEstate(int realEstateTypeId, int consultantId, int cityId, string title, string? description, 
            string? thumbnailUrl, string? externalSourceUrl, string? externalReferenceId, Money price, Address address, string status, bool isFeatured)
        {            
            RealEstateTypeId = realEstateTypeId;
            ConsultantId = consultantId;
            CityId = cityId;
            Title = title;
            Description = description;
            ThumbnailUrl = thumbnailUrl;
            ExternalSourceUrl = externalSourceUrl;
            ExternalReferenceId = externalReferenceId;
            Price = price ?? Price;
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
