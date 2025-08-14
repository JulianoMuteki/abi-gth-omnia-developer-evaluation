using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.EditSale;

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
        CreateMap<EditSaleCommand, Sale>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Items, opt => opt.Ignore());
        
        CreateMap<EditSaleItemCommand, SaleItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SaleId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        CreateMap<Sale, EditSaleResult>();
        CreateMap<SaleItem, EditSaleItemResult>();
    }
}
