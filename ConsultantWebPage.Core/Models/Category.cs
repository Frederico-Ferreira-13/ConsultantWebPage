using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
        public class Category : IEntity
    {
        [Key]
        public int CategoriesId { get; protected set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; protected set; } = string.Empty;

        public int CategoryTypeId { get; protected set; }
        public int? ParentCategoryId { get; protected set; }
        
        [ForeignKey("CategoryTypeId")]
        public virtual CategoryType? CategoryType { get; private set; }
        [ForeignKey("ParentCategoryId")]
        public virtual Category? ParentCategory { get; private set; }

        private Category() { }

        public Category(string categoryName, int categoryTypeId, int parentCategoryId)
        {
            ValidateCategory(categoryName, categoryTypeId, parentCategoryId);

            CategoryName = categoryName;
            CategoryTypeId = categoryTypeId;
            ParentCategoryId = parentCategoryId;
        }
        
        public void UpdateCategory(string categoryName, int categoryTypeId, int parentCategoryId)
        {
            ValidateCategory(categoryName, categoryTypeId, parentCategoryId);

            CategoryName = categoryName;
            CategoryTypeId = categoryTypeId;
            ParentCategoryId = parentCategoryId;
        }

        private void ValidateCategory(string categoryName, int categoryTypeId, int parentCategoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentException("Category name cannot be empty.");

            if (categoryTypeId <= 0)
                throw new ArgumentException("Category type ID must be a positive integer.");

            if (parentCategoryId < 0)
                throw new ArgumentException("Parent category ID cannot be negative.");
        }

        public int GetId() => CategoriesId;

        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be a positive integer.");

            CategoriesId = id;
        }

        public bool GetIsActive() => true; // Assuming categories are always active for this example
    }

}

