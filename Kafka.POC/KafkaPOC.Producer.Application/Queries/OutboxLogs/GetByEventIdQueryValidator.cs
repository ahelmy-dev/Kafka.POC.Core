
using FluentValidation;

namespace KafkaPOC.Producer.Application.Queries.OutboxLogs
{
    public class GetByEventIdQueryValidator : AbstractValidator<GetByEventIdQuery>
    {
        public GetByEventIdQueryValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty().WithMessage("Event ID cannot be empty.")
                .Must(BeAValidGuid).WithMessage("Invalid Event ID format.");
        }

        private bool BeAValidGuid(Guid eventId)
        {
            return eventId != Guid.Empty;
        }
    }
}