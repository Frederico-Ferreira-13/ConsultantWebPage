using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    public class Leads : IEntity
    {
        [Key]
        public int InquiryId { get; protected set; }

        public int RealEstateId { get; protected set; }


        [Required]
        [MaxLength(255)]
        public string ClientName { get; protected set; } = string.Empty;

        [MaxLength(255)]
        [EmailAddress]
        public string ClientEmail { get; protected set; } = string.Empty;

        [MaxLength(50)]
        public string ClientPhone { get; protected set; } = string.Empty;

        public string Message { get; protected set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; protected set; } = string.Empty;

        public DateTime CreatedAt { get; protected set; }

        [ForeignKey("RealEstateId")]
        public virtual RealEstate? RealEstate { get; private set; }

        private Leads() { }

        public Leads(int realEstateId, string clientName, string clientEmail, string clientPhone, string message)
        {
            ValidateLead(realEstateId, clientName, clientEmail, clientPhone);

            RealEstateId = realEstateId;
            ClientName = clientName;
            ClientEmail = clientEmail;
            ClientPhone = clientPhone;
            Message = message;
            Status = "New";
            CreatedAt = DateTime.UtcNow;
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

        public bool GetIsActive() => Status != "Closed"; // Assuming a lead is active unless its status is "Closed"

        private void ValidateLead(int realEstateId, string clientName, string clientEmail, string clientPhone)
        {
            if (realEstateId <= 0)
                throw new ArgumentException("RealEstateId must be a positive integer.");

            if (string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentException("Client name is required.");

            if (string.IsNullOrWhiteSpace(clientEmail) && string.IsNullOrWhiteSpace(clientPhone))
                throw new ArgumentException("At least one contact method (Email or Phone) must be provided.");
        }
    }

}

