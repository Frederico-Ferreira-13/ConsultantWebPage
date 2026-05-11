using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ConsultantWebPage.Core.Common;

namespace ConsultantWebPage.Core.Models;

public class Cities : IEntity
{
    [Key]
    public int CityId { get; protected set; }

    [Required]
    [MaxLength(100)]
    public string CityName { get; protected set; }

    [Required]
    [MaxLength(100)]
    public string DistrictName { get; protected set; } = string.Empty;

    public virtual ICollection<RealEstate> RealEstates { get; protected set; } = new List<RealEstate>();

    private Cities() { }

    private Cities(int cityId, string name, string districtName)
    {
        CityId = cityId;
        CityName = name;
        DistrictName = districtName;
    }     

    public static Result<Cities> Create(string name, string districtName)
    {
        if (string.IsNullOrWhiteSpace(name)) return Error.Validation("Nome da cidade é obrigatório.");
        if (name.Length > 100) return Error.Validation("Nome da cidade não pode exceder 100 caracteres.");
        if (string.IsNullOrWhiteSpace(districtName)) return Error.Validation("Nome do distrito é obrigatório.");
        if (districtName.Length > 100) return Error.Validation("Nome do distrito não pode exceder 100 caracteres.");

        return new Cities(0, name, districtName);
    }

    public int GetId() => CityId;

        public void SetId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID must be a positive integer.");

            CityId = id;
        }

        public bool GetIsActive() => true;
}
