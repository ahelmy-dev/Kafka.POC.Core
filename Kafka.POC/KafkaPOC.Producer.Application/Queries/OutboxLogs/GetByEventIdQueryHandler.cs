
using KafkaPOC.Producer.Application.DTOs;
using KafkaPOC.Producer.Application.Interfaces.OutboxEventLog;
using MediatR;

namespace KafkaPOC.Producer.Application.Queries.OutboxLogs
{
    public class GetByEventIdQueryHandler : IRequestHandler<GetByEventIdQuery, List<OutboxEventLogDto>>
    {
        private readonly IOutboxEventLogRepository _outboxEventLogRepository;

        public GetByEventIdQueryHandler(IOutboxEventLogRepository outboxEventLogRepository)
        {
            _outboxEventLogRepository = outboxEventLogRepository;
        }

        public async Task<List<OutboxEventLogDto>> Handle(GetByEventIdQuery request, CancellationToken cancellationToken)
        {
            var logs = await _outboxEventLogRepository.GetByEventIdAsync(request.EventId);
            return logs;
        }
    }
}