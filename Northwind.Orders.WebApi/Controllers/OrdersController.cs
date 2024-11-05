using Microsoft.AspNetCore.Mvc;
using Northwind.Orders.WebApi.Models;
using Northwind.Services.Repositories;

namespace Northwind.Orders.WebApi.Controllers;

[Route("api/orders")]
[ApiController]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderRepository orderRepository;
    private readonly ILogger<OrdersController> logger;

    public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
    {
        this.orderRepository = orderRepository;
        this.logger = logger;
    }

    public static void CheckPaginationParams(int skip, int count)
    {
        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip), $"Skip must be greater or equal to 0! Skip: {skip}");
        }
        else if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), $"Count must be greater than 0! Count: {count}");
        }
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<FullOrder>> GetOrderAsync(long orderId)
    {
        this.logger.LogTrace("Starting GetOrderAsync for orderId: {OrderId}", orderId);
        try
        {
            var order = await this.orderRepository.GetOrderAsync(orderId);
            this.logger.LogTrace("Successfully retrieved order for orderId: {OrderId}", orderId);
            return this.Ok(order.ToFullOrder());
        }
        catch (OrderNotFoundException ex)
        {
            this.logger.LogError(ex, "Order not found for orderId: {OrderId}", orderId);
            return new NotFoundResult();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred in GetOrderAsync for orderId: {OrderId}", orderId);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BriefOrder>>> GetOrdersAsync([FromQuery] int? skip, [FromQuery] int? count)
    {
        try
        {
            int toSkip = skip ?? 0;
            int toCount = count ?? 10;

            CheckPaginationParams(toSkip, toCount);

            this.logger.LogTrace("Starting GetOrdersAsync with skip: {Skip}, count: {Count}", skip, count);
            var orders = await this.orderRepository.GetOrdersAsync(toSkip, toCount);
            this.logger.LogTrace("Successfully retrieved orders with skip: {Skip}, count: {Count}", skip, count);
            return this.Ok(orders.Select(order => order.ToBriefOrder()));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            this.logger.LogError(ex, "Invalid pagination parameters");
            return new BadRequestResult();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred in GetOrdersAsync");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost]
    public async Task<ActionResult<AddOrder>> AddOrderAsync(BriefOrder order)
    {
        try
        {
            this.logger.LogTrace("Starting AddOrderAsync.");
            var orderId = await this.orderRepository.AddOrderAsync(order.ToOrder());
            this.logger.LogTrace("Order added successfully with orderId: {OrderId}", orderId);
            return this.Ok(new AddOrder() { OrderId = orderId });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred in AddOrderAsync");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("{orderId}")]
    public async Task<ActionResult> RemoveOrderAsync(long orderId)
    {
        this.logger.LogTrace("Starting RemoveOrderAsync for orderId: {OrderId}", orderId);
        try
        {
            await this.orderRepository.RemoveOrderAsync(orderId);
            this.logger.LogTrace("Order removed successfully for orderId: {OrderId}", orderId);
            return this.NoContent();
        }
        catch (OrderNotFoundException ex)
        {
            this.logger.LogError(ex, "Order not found for orderId: {OrderId}", orderId);
            return new NotFoundResult();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred in RemoveOrderAsync for orderId: {OrderId}", orderId);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("{orderId}")]
    public async Task<ActionResult> UpdateOrderAsync(long orderId, BriefOrder order)
    {
        this.logger.LogTrace("Starting UpdateOrderAsync for orderId: {OrderId}", orderId);
        try
        {
            if (order != null)
            {
                order.Id = orderId;
                await this.orderRepository.UpdateOrderAsync(order.ToOrder());
                this.logger.LogTrace("Order updated successfully for orderId: {OrderId}", orderId);
                return this.NoContent();
            }
            else
            {
                this.logger.LogTrace("Update is skipped for order body null with orderId: {OrderId}", orderId);
                return this.BadRequest("Order body cannot be null.");
            }
        }
        catch (OrderNotFoundException ex)
        {
            this.logger.LogError(ex, "Order not found for orderId: {OrderId}", orderId);
            return new NotFoundResult();
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "An unexpected error occurred in UpdateOrderAsync for orderId: {OrderId}", orderId);
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
