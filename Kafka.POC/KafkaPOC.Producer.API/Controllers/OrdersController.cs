using FluentValidation;
using KafkaPOC.Producer.Application.Commands.CreateOrder;
using KafkaPOC.Producer.Application.DTOs;
using KafkaPOC.Producer.Application.Queries.OrdersList;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KafkaPOC.Producer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<CreateOrderCommand> _validator;

        public OrdersController(IMediator mediator, IValidator<CreateOrderCommand> validator)
        {
            _mediator = mediator;
            _validator = validator;
        }

        /// <summary>
        /// Create Order
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var command = new CreateOrderCommand(dto);
            var validation = await _validator.ValidateAsync(command);

            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var orderId = await _mediator.Send(command);
            return Ok(new { OrderId = orderId });
        }

        /// <summary>
        /// Get All Orders 
        /// </summary>
        /// <param name="customerName"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? customerName,[FromQuery] int page = 1,[FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetOrdersQuery(customerName, page, pageSize));
            return Ok(result);
        }
    }
}