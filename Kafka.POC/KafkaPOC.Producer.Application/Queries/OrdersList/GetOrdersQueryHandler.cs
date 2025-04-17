
using KafkaPOC.Producer.Application.DTOs;
using KafkaPOC.Producer.Application.DTOs.Base;
using KafkaPOC.Producer.Domain.Repositories.Order;
using MediatR;

namespace KafkaPOC.Producer.Application.Queries.OrdersList
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PaginatedResult<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrdersQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<PaginatedResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var (orders, totalCount) = await _orderRepository.GetPagedAsync(
                request.CustomerName, request.Page, request.PageSize, cancellationToken);

            return new PaginatedResult<OrderDto>
            {
                Items = orders.Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerName = o.CustomerName,
                    CreationDate = o.CreationDate
                }).ToList(),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}