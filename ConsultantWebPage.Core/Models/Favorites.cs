using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    public class Favorites : IEntity
    {
    [Key]
        public int FavoritesId { get; protected set; }

        public int UserId { get; protected set; }
        public int RealEstateId { get; protected set; }
        public DateTime CreatedAt { get; protected set; }

        [ForeignKey("UserId")]
        public virtual Users? User { get; private set; }

        [ForeignKey("RealEstateId")]
        public virtual RealEstate? RealEstate { get; private set; }

        private Favorites() { }

        [SetsRequiredMembers]
        public Favorites(int userId, int realEstateId)
        {
            ValidateFavorite(userId, realEstateId);

            UserId = userId;
            RealEstateId = realEstateId;
            CreatedAt = DateTime.UtcNow;
        }

        // Construtor para preenchimento manual ou legado
        public Favorites(int favoritesId, int userId, int realEstateId, DateTime createdAt)
        {
            if(favoritesId <= 0)
                throw new ArgumentException("O ID não pode ser negativo.", nameof(favoritesId));

            FavoritesId = favoritesId;
            UserId = userId;
            RealEstateId = realEstateId;
            CreatedAt = createdAt;
        }

        public bool IsValid()
        {
            return UserId > 0 && RealEstateId > 0;
        }

        public int GetId() => FavoritesId;

        public void SetId(int id)
        {
            if (FavoritesId != 0)
            {
                throw new InvalidOperationException("Não é permitido alterar o ID de uma Entidade que já possui um ID.");
            }
            FavoritesId = id;
        }  

        private void ValidateFavorite(int userId, int realEstateId)
        {
            if (userId <= 0)
                throw new ArgumentException("O ID do usuário deve ser um inteiro positivo.", nameof(userId));

            if (realEstateId <= 0)
                throw new ArgumentException("O ID do imóvel deve ser um inteiro positivo.", nameof(realEstateId));
        }

        public bool GetIsActive() => true; 
    }
}
