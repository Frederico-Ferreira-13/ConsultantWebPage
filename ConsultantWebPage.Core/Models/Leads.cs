using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;
using ConsultantWebPage.Core.Models.ValueObjects;

namespace ConsultantWebPage.Core.Models
{
    public class Leads : IEntity
    {
        [Key]
        public int InquiryId { get; protected set; }

        public int? RealEstateId { get; protected set; }
        public int? ConsultantId { get; protected set; }

        [Required]
        [MaxLength(255)]
        public string ClientName { get; protected set; } = string.Empty;

        public ContactInfo Contact { get; protected set; } = null!; 

        public string Message { get; protected set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; protected set; } = string.Empty;

        public DateTime CreatedAt { get; protected set; }

        [ForeignKey("RealEstateId")]
        public virtual RealEstate? RealEstate { get; private set; }

        private Leads() { }

        private Leads(int? realEstateId, int? consultantId, string clientName, ContactInfo contact, string message)
        {
            RealEstateId = realEstateId;
            ConsultantId = consultantId;
            ClientName = clientName;
            Contact = contact;
            Message = message;
            Status = "Novo";
            CreatedAt = DateTime.UtcNow;
        }

        public static Result<Leads> Create(int? realEstateId, int? consultantId, string clientName, ContactInfo contact, string message)
        {
            if (string.IsNullOrWhiteSpace(clientName)) return Error.Validation("Nome do cliente obrigatório.");
            if (contact == null) return Error.Validation("Informação de contacto obrigatória.");
                        
            return new Leads(realEstateId, consultantId, clientName, contact, message);
        }

        public void AssignToConsultant(int consultantId)
        {
            if (consultantId <= 0) throw new ArgumentException("ID de consultor inválido.");
            ConsultantId = consultantId;
        }

        public void UpdateStatus(string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                throw new ArgumentException("Status cannot be empty.");

            Status = newStatus;
        }

        public int GetId() => InquiryId;

        public void SetId(int id)
        {
            if (InquiryId != 0)
            {
                throw new InvalidOperationException("Cannot change the ID of an entity that already has an ID.");
            }
            InquiryId = id;
        }

        public bool GetIsActive() => Status != "Fechado" && Status != "Closed";
    }
}
