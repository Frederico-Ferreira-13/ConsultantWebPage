using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    [Table("Category")]
    public class Category : IEntity
    {
        [Key]
        public int CategoriesId { get; protected set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; protected set; } = string.Empty;

        [Required]
        public int CategoryTypeId { get; protected set; }

        public int? ParentCategoryId { get; protected set; }
        
        [ForeignKey("CategoryTypeId")]
        public virtual CategoryType? CategoryType { get; private set; }

        [ForeignKey("ParentCategoryId")]
        public virtual Category? ParentCategory { get; private set; }

        public virtual ICollection<Category> SubCategories { get; private set; } = new List<Category>();

        private Category() { }

        public Category(string categoryName, int categoryTypeId, int? parentCategoryId = null)
        {
            ValidateCategory(categoryName, categoryTypeId, parentCategoryId);

            CategoryName = categoryName;
            CategoryTypeId = categoryTypeId;
            ParentCategoryId = parentCategoryId;
        }
        
        public void UpdateCategory(string categoryName, int categoryTypeId, int? parentCategoryId = null)
        {
            ValidateCategory(categoryName, categoryTypeId, parentCategoryId);

            CategoryName = categoryName;
            CategoryTypeId = categoryTypeId;
            ParentCategoryId = parentCategoryId;
        }

        private void ValidateCategory(string categoryName, int categoryTypeId, int? parentCategoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentException("Category name cannot be empty.");

            if (categoryTypeId <= 0)
                throw new ArgumentException("Category type ID must be a positive integer.");

            // Se houver pai, o ID tem de ser válido
            if (parentCategoryId.HasValue && parentCategoryId.Value <= 0)
                throw new ArgumentException("Parent category ID must be a positive integer if provided.");
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
