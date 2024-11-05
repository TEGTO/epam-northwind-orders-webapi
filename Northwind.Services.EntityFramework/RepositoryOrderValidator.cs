using Northwind.Services.Repositories;
using RepositoryOrder = Northwind.Services.Repositories.Order;

namespace Northwind.Services.EntityFramework;
public static class RepositoryOrderValidator
{
    public static void ValidateOrder(RepositoryOrder repoOrder)
    {
        if (repoOrder == null)
        {
            throw new RepositoryException("Order cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(repoOrder.Customer?.Code.Code))
        {
            throw new RepositoryException("Customer ID is required.");
        }

        if (repoOrder.Employee == null || repoOrder.Employee.Id <= 0)
        {
            throw new RepositoryException("Valid Employee ID is required.");
        }

        if (repoOrder.OrderDate == default)
        {
            throw new RepositoryException("Order date is required.");
        }

        if (repoOrder.RequiredDate <= repoOrder.OrderDate)
        {
            throw new RepositoryException("Required date must be after the order date.");
        }

        if (repoOrder.Freight < 0)
        {
            throw new RepositoryException("Freight must be non-negative.");
        }

        if (string.IsNullOrWhiteSpace(repoOrder.ShipName))
        {
            throw new RepositoryException("Ship name is required.");
        }

        if (repoOrder.OrderDetails == null || !repoOrder.OrderDetails.Any())
        {
            throw new RepositoryException("At least one order detail is required.");
        }

        foreach (var detail in repoOrder.OrderDetails)
        {
            if (detail.Product.Id <= 0)
            {
                throw new RepositoryException("Valid Product ID is required.");
            }

            if (detail.Quantity <= 0)
            {
                throw new RepositoryException("Order detail quantity must be greater than zero.");
            }

            if (detail.UnitPrice < 0)
            {
                throw new RepositoryException("Order detail unit price must be non-negative.");
            }

            if (detail.Discount < 0 || detail.Discount > 1)
            {
                throw new RepositoryException("Order detail discount must be between 0 and 1.");
            }
        }
    }
}
