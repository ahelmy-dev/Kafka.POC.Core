using System.Text.Json;
using KafkaPOC.Producer.Domain.Entities.Order;
using KafkaPOC.Producer.Domain.Entities.OutboxEvent;
using KafkaPOC.Producer.Domain.Repositories;
using MediatR;

namespace KafkaPOC.Producer.Application.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var dto = command.Dto;

            var order = new Order(dto.CustomerName);
            await _unitOfWork.Orders.AddAsync(order);

            var payload = JsonSerializer.Serialize(order);
            var outbox = new OutboxEvent("OrderCreated", payload,order.Id,dto.CustomerName);
            await _unitOfWork.OutboxEvents.AddAsync(outbox);

            await _unitOfWork.CommitAsync();
            return order.Id;
        }
    }
}