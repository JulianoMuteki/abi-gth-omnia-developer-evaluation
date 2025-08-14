using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

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
        CreateMap<DeleteSaleCommand, Sale>();
    }
}
