using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// AutoMapper profile for GetSales operations
/// </summary>
public class GetSalesProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of GetSalesProfile
    /// </summary>
    public GetSalesProfile()
    {
        CreateMap<Sale, GetSalesItemResult>()
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.Items.Count));
    }
}
