using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    public class Testimonials : IEntity
    {
        [Key]
        public int TestimonialId { get; protected set; }
        public int ConsultantId { get; protected set; }

        [Required]
        [MaxLength(255)]
        public string ClientName { get; protected set; } = string.Empty;

        [Required]
        public string TestimonialText { get; protected set; } = string.Empty;

        public int Rating { get; protected set; }
        public bool IsVisible { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        [ForeignKey("ConsultantId")]
        public virtual Users? Consultant { get; private set; }

        private Testimonials() { }

        public Testimonials(int consultantId, string? clientName, string? testimonialText, int rating, bool isVisible)
        {
            ValidateTestimonial(consultantId, rating);

            ConsultantId = consultantId;
            ClientName = clientName;
            TestimonialText = testimonialText;
            Rating = rating;
            IsVisible = isVisible;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateVisibility(bool isVisible)
        {
            IsVisible = isVisible;
        }

        private void ValidateTestimonial(int consultantId, int rating)
        {
            if (consultantId <= 0)
                throw new ArgumentException("Consultant ID must be a positive integer.");

            
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");
        }

        public int GetId() => TestimonialId;

        public void SetId(int id)
        {
            if (TestimonialId != 0)
                throw new InvalidOperationException("Não é permitido alterar o ID de um testemunho existente.");

            if (id <= 0)
                throw new ArgumentException("ID must be a positive integer.");

            TestimonialId = id;
        }

        public bool GetIsActive() => IsVisible;
    }
}
