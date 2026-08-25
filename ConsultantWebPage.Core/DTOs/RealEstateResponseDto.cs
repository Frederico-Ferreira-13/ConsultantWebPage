using System.Collections.Generic;

namespace ConsultantWebPage.Core.Models.DTOs;

public class RealEstateResponseDto
{
    public int RealEstateId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
    public string TypeName { get; set; } // Apenas o nome, não o objeto completo
    public string ConsultantName { get; set; }
}