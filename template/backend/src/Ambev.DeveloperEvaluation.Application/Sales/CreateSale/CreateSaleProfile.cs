using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// AutoMapper profile for CreateSale operations
/// </summary>
public class CreateSaleProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of CreateSaleProfile
    /// </summary>
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleCommand, Sale>();
        CreateMap<CreateSaleItemCommand, SaleItem>();
        CreateMap<Sale, CreateSaleResult>();
        CreateMap<SaleItem, CreateSaleItemResult>();
    }
}
