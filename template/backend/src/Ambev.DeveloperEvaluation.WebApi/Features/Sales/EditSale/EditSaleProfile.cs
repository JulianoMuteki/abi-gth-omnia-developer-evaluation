using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.EditSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.EditSale;

/// <summary>
/// AutoMapper profile for EditSale operations
/// </summary>
public class EditSaleProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of EditSaleProfile
    /// </summary>
    public EditSaleProfile()
    {
        CreateMap<EditSaleRequest, EditSaleCommand>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<EditSaleItemRequest, EditSaleItemCommand>();
        CreateMap<EditSaleResult, EditSaleResponse>();
        CreateMap<EditSaleItemResult, EditSaleItemResponse>();
    }
}
