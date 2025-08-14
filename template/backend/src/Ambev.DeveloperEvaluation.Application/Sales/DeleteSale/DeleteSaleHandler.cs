using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Handler for DeleteSaleCommand
/// </summary>
public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResult>
{
    private readonly ISaleRepository _saleRepository;

    /// <summary>
    /// Initializes a new instance of DeleteSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    public DeleteSaleHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    /// <summary>
    /// Handles the DeleteSaleCommand
    /// </summary>
    /// <param name="request">The delete sale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The delete sale result</returns>
    public async Task<DeleteSaleResult> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {request.Id} not found.");

        await _saleRepository.DeleteAsync(sale, cancellationToken);
        await _saleRepository.SaveChangesAsync(cancellationToken);

        return new DeleteSaleResult
        {
            Success = true
        };
    }
}
