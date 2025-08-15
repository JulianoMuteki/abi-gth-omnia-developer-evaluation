using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using AutoMapper;
using FluentValidation;
using System.Linq;

namespace Ambev.DeveloperEvaluation.Application.Sales.EditSale;

/// <summary>
/// Handler for EditSaleCommand
/// </summary>
public class EditSaleHandler : IRequestHandler<EditSaleCommand, EditSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IPublisher _publisher;

    /// <summary>
    /// Initializes a new instance of EditSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="publisher">The MediatR publisher for domain events</param>
    public EditSaleHandler(ISaleRepository saleRepository, IMapper mapper, IPublisher publisher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _publisher = publisher;
    }

    /// <summary>
    /// Handles the EditSaleCommand
    /// </summary>
    /// <param name="command">The edit sale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The edit sale result</returns>
    public async Task<EditSaleResult> Handle(EditSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new EditSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingSale == null)
            throw new InvalidOperationException($"Sale with ID {command.Id} not found");

        if (existingSale.SaleNumber != command.SaleNumber)
        {
            var saleWithSameNumber = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
            if (saleWithSameNumber != null)
                throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");
        }

        _mapper.Map(command, existingSale);
        existingSale.UpdatedAt = DateTime.UtcNow;

        foreach (var itemCommand in command.Items)
        {
            if (itemCommand.Id.HasValue)
            {
                var existingItem = existingSale.Items.FirstOrDefault(i => i.Id == itemCommand.Id.Value);
                if (existingItem != null)
                {
                    _mapper.Map(itemCommand, existingItem);
                    existingItem.CalculateTotalAmount();
                }
                else
                {
                    throw new InvalidOperationException($"Sale item with ID {itemCommand.Id.Value} not found in sale {command.Id}");
                }
            }
            else
            {
                var saleItem = _mapper.Map<SaleItem>(itemCommand);
                saleItem.SaleId = existingSale.Id;
                saleItem.CalculateTotalAmount();
                existingSale.Items.Add(saleItem);
            }
        }

        existingSale.CalculateTotalAmount();

        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);
        
        // Publish domain event
        var saleModifiedEvent = new SaleModifiedEvent(updatedSale);
        await _publisher.Publish(saleModifiedEvent, cancellationToken);
        
        var result = _mapper.Map<EditSaleResult>(updatedSale);
        return result;
    }
}
