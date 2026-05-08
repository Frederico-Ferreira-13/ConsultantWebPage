using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    public class RealEstateCategories : IEntity
    {
        // Chaves compostas não podem ser definidas apenas com [Key] no C#
        // Teremos que configurar isto no DbContext (OnModelCreating)
        public int RealEstateId { get; protected set; }
        public int CategoriesId { get; protected set; }

        [ForeignKey("RealEstateId")]
        public virtual RealEstate? RealEstate { get; private set; }

        [ForeignKey("CategoriesId")]
        public virtual Category? Category { get; private set; }

        private RealEstateCategories() { }

        public RealEstateCategories(int realEstateId, int categoriesId)
        {
            ValidateRealEstateCategory(realEstateId, categoriesId);

            RealEstateId = realEstateId;
            CategoriesId = categoriesId;
        }

        private void ValidateRealEstateCategory(int realEstateId, int categoriesId)
        {
            if (realEstateId <= 0)
                throw new ArgumentException("Real estate ID must be a positive integer.");

            if (categoriesId <= 0)
                throw new ArgumentException("Category ID must be a positive integer.");
        }

        public int GetId() => RealEstateId;

        public void SetId(int id)
        {
            RealEstateId = id;
        }

        public bool GetIsActive() => true;
    }
}
