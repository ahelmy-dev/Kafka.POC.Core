using KafkaPOC.Producer.Application.DTOs;
using MediatR;

namespace KafkaPOC.Producer.Application.Queries.OutboxLogs
{
    public class GetByEventIdQuery : IRequest<List<OutboxEventLogDto>>
    {
        public Guid EventId { get; }

        public GetByEventIdQuery(Guid eventId)
        {
            EventId = eventId;
        }
    }
}