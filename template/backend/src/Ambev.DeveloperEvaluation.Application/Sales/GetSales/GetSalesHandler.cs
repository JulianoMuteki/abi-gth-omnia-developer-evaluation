using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Handler for processing GetSalesCommand requests
/// </summary>
public class GetSalesHandler : IRequestHandler<GetSalesCommand, GetSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetSalesHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public GetSalesHandler(
        ISaleRepository saleRepository,
        IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetSalesCommand request
    /// </summary>
    /// <param name="request">The GetSales command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The paginated sales list</returns>
    public async Task<GetSalesResult> Handle(GetSalesCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetSalesCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var (sales, totalCount) = await _saleRepository.GetPaginatedAsync(request.PageNumber,
                                                                            request.PageSize,
                                                                            request.CustomerId,
                                                                            request.BranchId,
                                                                            request.StartDate,
                                                                            request.EndDate,
                                                                            request.Status,
                                                                            cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var salesItems = _mapper.Map<List<GetSalesItemResult>>(sales);

        return new GetSalesResult
        {
            Sales = salesItems,
            CurrentPage = request.PageNumber,
            TotalPages = totalPages,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
