
using KafkaPOC.Producer.Application.DTOs;
using KafkaPOC.Producer.Application.DTOs.Base;
using MediatR;

namespace KafkaPOC.Producer.Application.Queries.OrdersList
{
    public class GetOrdersQuery : IRequest<PaginatedResult<OrderDto>>
    {
        public string? CustomerName { get; }
        public int Page { get; }
        public int PageSize { get; }

        public GetOrdersQuery(string? customerName, int page, int pageSize)
        {
            CustomerName = customerName;
            Page = page < 1 ? 1 : page;
            PageSize = pageSize < 1 ? 10 : pageSize;
        }
    }
}