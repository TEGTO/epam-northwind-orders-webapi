using Microsoft.EntityFrameworkCore;
using Northwind.Services.EntityFramework.Entities;
using Northwind.Services.Repositories;
using Order = Northwind.Services.EntityFramework.Entities.Order;
using OrderDetail = Northwind.Services.EntityFramework.Entities.OrderDetail;
using RepositoryOrder = Northwind.Services.Repositories.Order;
using RepositoryOrderDetail = Northwind.Services.Repositories.OrderDetail;

namespace Northwind.Services.EntityFramework.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly NorthwindContext context;

    public OrderRepository(NorthwindContext context)
    {
        this.context = context;
    }

    public async Task<RepositoryOrder> GetOrderAsync(long orderId)
    {
        var order = await this.GetOrderQueryable().FirstOrDefaultAsync(o => o.OrderID == orderId).SetConfigureAwait();

        if (order == null)
        {
            throw new OrderNotFoundException("Order is not found!");
        }

        return order.ToRepositoryOrder();
    }

    public async Task<IList<RepositoryOrder>> GetOrdersAsync(int skip, int count)
    {
        OrderRepositoryExtenstions.CheckPaginationParams(skip, count);

        var orders = await this.GetOrderQueryable()
            .Skip(skip)
            .Take(count)
            .OrderBy(x => x.OrderID)
            .ToListAsync()
            .SetConfigureAwait();

        return orders.Select(order => order.ToRepositoryOrder()).ToArray();
    }

    public async Task<long> AddOrderAsync(RepositoryOrder order)
    {
        RepositoryOrderValidator.ValidateOrder(order);

        var orderEntity = await order.ToOrderAsync(this.context).SetConfigureAwait();
        _ = await this.context.Orders.AddAsync(orderEntity);
        _ = await this.context.SaveChangesAsync();
        return orderEntity.OrderID;
    }

    public async Task RemoveOrderAsync(long orderId)
    {
        var orderEntity = await this.GetOrderQueryable().FirstOrDefaultAsync(o => o.OrderID == orderId).SetConfigureAwait();
        if (orderEntity == null)
        {
            throw new OrderNotFoundException("Order is not found!");
        }

        _ = this.context.Orders.Remove(orderEntity);
        _ = await this.context.SaveChangesAsync();
    }

    public async Task UpdateOrderAsync(RepositoryOrder order)
    {
        ExceptionHelper.ThrowArgumentNullException(order);

        var existingOrder = await this.LoadExistingOrderAsync(order.Id).SetConfigureAwait();
        if (existingOrder == null)
        {
            throw new OrderNotFoundException("Order is not found!");
        }

        OrderRepositoryExtenstions.UpdateOrderProperties(existingOrder, order);
        await this.UpdateOrderDetailsAsync(existingOrder, order.OrderDetails).SetConfigureAwait();

        _ = await this.context.SaveChangesAsync().SetConfigureAwait();
    }

    private async Task<Order?> LoadExistingOrderAsync(long orderId)
    {
        return await this.context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OrderID == orderId)
            .SetConfigureAwait();
    }

    private async Task UpdateOrderDetailsAsync(Order existingOrder, IList<RepositoryOrderDetail> updatedOrderDetails)
    {
        var newOrderDetails = new List<OrderDetail>();

        foreach (var repoOrderDetail in updatedOrderDetails)
        {
            var existingDetail = existingOrder.OrderDetails
                .FirstOrDefault(od => od.ProductID == repoOrderDetail.Product.Id);

            if (existingDetail != null)
            {
                OrderRepositoryExtenstions.UpdateExistingOrderDetail(existingDetail, repoOrderDetail);
            }
            else
            {
                var newDetail = await repoOrderDetail.ToOrderDetailAsync(existingOrder, this.context).SetConfigureAwait();
                newOrderDetails.Add(newDetail);
            }
        }

        this.RemoveDeletedOrderDetails(existingOrder, updatedOrderDetails);
        OrderRepositoryExtenstions.AddNewOrderDetails(existingOrder, newOrderDetails);
    }

    private void RemoveDeletedOrderDetails(Order existingOrder, IList<RepositoryOrderDetail> updatedOrderDetails)
    {
        var detailsToRemove = existingOrder.OrderDetails
            .Where(od => !updatedOrderDetails.Any(repoOd => repoOd.Product.Id == od.ProductID))
            .ToList();

        foreach (var detail in detailsToRemove)
        {
            _ = this.context.OrderDetails.Remove(detail);
        }
    }

    private IQueryable<Order> GetOrderQueryable()
    {
        return this.context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .Include(o => o.Shipper)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .ThenInclude(p => p.Category)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .ThenInclude(p => p.Supplier);
    }
}

public static class OrderRepositoryExtenstions
{
    public static void AddNewOrderDetails(Order existingOrder, IList<OrderDetail> newOrderDetails)
    {
        ExceptionHelper.ThrowArgumentNullException(newOrderDetails);
        ExceptionHelper.ThrowArgumentNullException(existingOrder);

        foreach (var newDetail in newOrderDetails)
        {
            existingOrder.OrderDetails.Add(newDetail);
        }
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

    public static void UpdateExistingOrderDetail(OrderDetail existingDetail, RepositoryOrderDetail updatedDetail)
    {
        ExceptionHelper.ThrowArgumentNullException(updatedDetail);
        ExceptionHelper.ThrowArgumentNullException(existingDetail);

        existingDetail.Quantity = updatedDetail.Quantity;
        existingDetail.UnitPrice = updatedDetail.UnitPrice;
        existingDetail.Discount = updatedDetail.Discount;
    }

    public static void UpdateOrderProperties(Order existingOrder, RepositoryOrder updatedOrder)
    {
        ExceptionHelper.ThrowArgumentNullException(updatedOrder);
        ExceptionHelper.ThrowArgumentNullException(existingOrder);

        existingOrder.CustomerID = updatedOrder.Customer.Code.Code!;
        existingOrder.EmployeeID = updatedOrder.Employee.Id;
        existingOrder.OrderDate = updatedOrder.OrderDate;
        existingOrder.RequiredDate = updatedOrder.RequiredDate;
        existingOrder.ShippedDate = updatedOrder.ShippedDate;
        existingOrder.ShipVia = updatedOrder.Shipper.Id;
        existingOrder.Freight = updatedOrder.Freight;
        existingOrder.ShipName = updatedOrder.ShipName;
        existingOrder.ShipAddress = updatedOrder.ShippingAddress.Address;
        existingOrder.ShipCity = updatedOrder.ShippingAddress.City;
        existingOrder.ShipRegion = updatedOrder.ShippingAddress.Region;
        existingOrder.ShipPostalCode = updatedOrder.ShippingAddress.PostalCode;
        existingOrder.ShipCountry = updatedOrder.ShippingAddress.Country;
    }
}
