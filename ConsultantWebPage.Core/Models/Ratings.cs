using System;
using System.Diagnostics.CodeAnalysis;
using ConsultantWebPage.Core.Models.ValueObjects;
using System.ComponentModel.DataAnnotations;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    public class Ratings : IEntity
    {
        public int RatingsId { get; protected set; }
        public int RealEstateId { get; protected set; }
        public int UserId { get; protected set; }        
        public DateTime CreatedAt { get; protected set; }

        public virtual RealEstate? RealEstate { get; private set; }
        public virtual Users? User { get; private set; }

        public StarRating? RatingValue { get; protected set; }

        private const int MinRating = 1;
        private const int MaxRating = 5;

        [SetsRequiredMembers]
        private Ratings()
        {
            RatingsId = default;
        }

        public Ratings(int realEstateId, int userId, int ratingValue)
        {
            ValidateRating(realEstateId, userId);

            RatingsId = default;

            RealEstateId = realEstateId;
            UserId = userId;
            RatingValue = StarRating.Create(ratingValue);
            this.CreatedAt = DateTime.UtcNow;
        }

        public Ratings(int id, DateTime createdAt,
            int realEstateId, int userId, StarRating ratingValue)
        {
            RatingsId = id;

            RealEstateId = realEstateId;
            UserId = userId;
            RatingValue = ratingValue;
            CreatedAt = createdAt;
        }

        public void UpdateRating(int newRatingValue)
        {
            ValidateRatingValue(newRatingValue);

            var newRating = StarRating.Create(newRatingValue);

            if (!RatingValue.Equals(newRating))
            {
                RatingValue = newRating;
            }
        }

        private static void ValidateRating(int recipesId, int userId)
        {
            if (recipesId <= 0)
            {
                throw new ArgumentException("O ID da Receita deve ser positivo.", nameof(recipesId));
            }
            if (userId <= 0)
            {
                throw new ArgumentException("O ID do Utilizador deve ser positivo.", nameof(userId));
            }
        }

        private static void ValidateRatingValue(int ratingValue)
        {
            if (ratingValue < MinRating || ratingValue > MaxRating)
            {
                throw new ArgumentOutOfRangeException(nameof(ratingValue),
                    $"A avaliação deve estar entre {MinRating} e {MaxRating} estrelas.");
            }
        }

        public int GetId() => RatingsId;

        public void SetId(int id)
        {
            if (RatingsId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade que já possui um ID.");
            }
            RatingsId = id;
        }

        public bool GetIsActive() => true;
    }
}
