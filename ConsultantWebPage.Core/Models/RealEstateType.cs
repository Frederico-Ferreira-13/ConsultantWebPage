using System;
using System.ComponentModel.DataAnnotations;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models
{
    public class RealEstateType : IEntity
    {
        [Key]
        public int RealEstateTypeId { get; protected set; }

        [Required]
        [MaxLength(100)]
        public string TypeName { get; protected set; } = string.Empty;

        private RealEstateType() { }

        public RealEstateType(string typeName)
        {
            ValidateRealEstateType(typeName);

            TypeName = typeName;
        }

        public void UpdateRealEstateType(string typeName)
        {
            ValidateRealEstateType(typeName);
            TypeName = typeName;
        }
        private void ValidateRealEstateType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
                throw new ArgumentException("Real estate type name cannot be empty.");
        }

        public int GetId() => RealEstateTypeId;

        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be a positive integer.");

            RealEstateTypeId = id;
        }

        public bool GetIsActive() => true;
    }
}
