using FluentValidation;

namespace KafkaPOC.Producer.Application.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.Dto.CustomerName)
                .NotEmpty().WithMessage("Customer name is required")
                .MinimumLength(3).WithMessage("Customer name must be at least 3 characters");
        }
    }
}