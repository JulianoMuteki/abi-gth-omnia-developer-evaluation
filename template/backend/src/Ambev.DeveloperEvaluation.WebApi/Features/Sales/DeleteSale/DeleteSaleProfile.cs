using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

/// <summary>
/// AutoMapper profile for DeleteSale operations
/// </summary>
public class DeleteSaleProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of DeleteSaleProfile
    /// </summary>
    public DeleteSaleProfile()
    {
        CreateMap<DeleteSaleRequest, DeleteSaleCommand>();
    }
}
