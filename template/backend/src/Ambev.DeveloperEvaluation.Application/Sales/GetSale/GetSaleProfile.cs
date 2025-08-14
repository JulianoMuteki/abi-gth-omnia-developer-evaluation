using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// AutoMapper profile for GetSale operations
/// </summary>
public class GetSaleProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of GetSaleProfile
    /// </summary>
    public GetSaleProfile()
    {
        CreateMap<GetSaleCommand, Sale>();
        CreateMap<Sale, GetSaleResult>();
        CreateMap<SaleItem, GetSaleItemResult>();
    }
}
