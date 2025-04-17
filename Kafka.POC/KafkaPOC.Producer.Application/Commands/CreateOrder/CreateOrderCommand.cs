using KafkaPOC.Producer.Application.DTOs;
using MediatR;

namespace KafkaPOC.Producer.Application.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid>
    {
        public CreateOrderDto Dto { get; }

        public CreateOrderCommand(CreateOrderDto dto)
        {
            Dto = dto;
        }
    }
}