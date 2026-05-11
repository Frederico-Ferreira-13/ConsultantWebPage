using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    public class PropertyEvaluations : IEntity
    {
        [Key]
        public int EvaluationId { get; protected set; }

        public int ConsultantId { get; protected set; }

        public int? UserId { get; protected set; }

        [Required]
        [MaxLength(255)]
        public string ClientName { get; protected set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string ClientContact { get; protected set; } = string.Empty;

        [MaxLength(100)]
        public string PropertyType { get; protected set; } = string.Empty;

        public string Address { get; protected set; } = string.Empty;

        [MaxLength(50)]
        public string Typology { get; protected set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Area { get; protected set; }

        [MaxLength(100)]
        public string Condition { get; protected set; } = string.Empty;

        [MaxLength(50)]
        public string Status { get; protected set; } = string.Empty;

        public DateTime CreatedAt { get; protected set; }

        [ForeignKey("ConsultantId")]
        public virtual Users? Consultant { get; private set; }
        
        [ForeignKey("UserId")]
        public virtual Users? Requester { get; private set; }

        private PropertyEvaluations() { }

        public PropertyEvaluations(int consultantId, string clientName, string clientContact, string propertyType,
            string address, string typology, decimal area, string condition, int? userId = null)
        {
            ValidateEvaluation(consultantId, clientName, clientContact, area, userId);

            ConsultantId = consultantId;
            UserId = userId;
            ClientName = clientName;
            ClientContact = clientContact;
            PropertyType = propertyType;
            Address = address;
            Typology = typology;
            Area = area;
            Condition = condition;
            Status = "Pending";
            CreatedAt = DateTime.UtcNow;
        }

       private void ValidateEvaluation(int consultantId, string? clientName, string? clientContact, decimal area, int? userId)
        {
            if (consultantId <= 0)
                throw new ArgumentException("Consultant ID must be a positive integer.");
            
            // Se não for um utilizador registado, o nome e contacto são obrigatórios
            if (!userId.HasValue && string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentException("Client name is required for guest evaluations.");

            if (area <= 0)
                throw new ArgumentException("Area must be a positive number.");
        }

        public void UpdateStatus(string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                throw new ArgumentException("Status cannot be empty.");

            Status = newStatus;
        }

        public int GetId() => EvaluationId;

        public void SetId(int id)
        {
            if (EvaluationId != 0)
                throw new InvalidOperationException("Cannot change the ID of an entity.");
            EvaluationId = id;
        }

        public bool GetIsActive() => Status != "Completed";
    }
}
