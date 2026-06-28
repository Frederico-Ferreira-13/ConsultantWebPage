using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    [Table("CategoryType")]
    public class CategoryType : IEntity
    {
        [Key]
        public int CategoryTypeId { get; protected set; }

        [Required]
        [MaxLength(100)]
        public string TypeName { get; protected set; } = string.Empty;

        private CategoryType() { }

        public CategoryType(string typeName)
        {
            ValidateCategoryType(typeName);
            TypeName = typeName;
        }

        public void UpdateCategoryType(string typeName)
        {
            ValidateCategoryType(typeName);
            TypeName = typeName;
        }

        private void ValidateCategoryType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException("Category type name cannot be empty.");
        }

        public int GetId() => CategoryTypeId;

        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be a positive integer.");

            CategoryTypeId = id;
        }

        public bool GetIsActive() => true; // Assuming CategoryType is always active, adjust as needed
    } 
}
