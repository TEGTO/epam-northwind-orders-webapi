using Microsoft.EntityFrameworkCore;
using Northwind.Services.EntityFramework.Entities;
using Northwind.Services.Repositories;
using Customer = Northwind.Services.EntityFramework.Entities.Customer;
using Employee = Northwind.Services.EntityFramework.Entities.Employee;
using Order = Northwind.Services.EntityFramework.Entities.Order;
using OrderDetail = Northwind.Services.EntityFramework.Entities.OrderDetail;
using Product = Northwind.Services.EntityFramework.Entities.Product;
using RepositoryCustomer = Northwind.Services.Repositories.Customer;
using RepositoryEmployee = Northwind.Services.Repositories.Employee;
using RepositoryOrder = Northwind.Services.Repositories.Order;
using RepositoryOrderDetail = Northwind.Services.Repositories.OrderDetail;
using RepositoryProduct = Northwind.Services.Repositories.Product;
using RepositoryShipper = Northwind.Services.Repositories.Shipper;
using Shipper = Northwind.Services.EntityFramework.Entities.Shipper;

namespace Northwind.Services.EntityFramework;
public static class OrderMappingExtensions
{
    public static RepositoryOrder ToRepositoryOrder(this Order order)
    {
        ExceptionHelper.ThrowArgumentNullException(order);

        var repositoryOrder = new RepositoryOrder(order.OrderID)
        {
            Customer = order.Customer.ToRepositoryCustomer(),
            Employee = order.Employee.ToRepositoryEmployee(),
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            Shipper = order.Shipper.ToRepositoryShipper(),
            Freight = order.Freight,
            ShipName = order.ShipName,
            ShippingAddress = new ShippingAddress(
                order.ShipAddress,
                order.ShipCity,
                order.ShipRegion,
                order.ShipPostalCode,
                order.ShipCountry),
        };

        foreach (var orderDetail in order.OrderDetails)
        {
            repositoryOrder.OrderDetails.Add(orderDetail.ToRepositoryOrderDetail(repositoryOrder));
        }

        return repositoryOrder;
    }

    public static RepositoryCustomer ToRepositoryCustomer(this Customer customer)
    {
        ExceptionHelper.ThrowArgumentNullException(customer);

        return new RepositoryCustomer(new CustomerCode(customer.CustomerID))
        {
            CompanyName = customer.CompanyName,
        };
    }

    public static RepositoryEmployee ToRepositoryEmployee(this Employee employee)
    {
        ExceptionHelper.ThrowArgumentNullException(employee);

        return new RepositoryEmployee(employee.EmployeeID)
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Country = employee.Country,
        };
    }

    public static RepositoryShipper ToRepositoryShipper(this Shipper shipper)
    {
        ExceptionHelper.ThrowArgumentNullException(shipper);

        return new RepositoryShipper(shipper.ShipperID)
        {
            CompanyName = shipper.CompanyName,
        };
    }

    public static RepositoryOrderDetail ToRepositoryOrderDetail(this OrderDetail orderDetail, RepositoryOrder repositoryOrder)
    {
        ExceptionHelper.ThrowArgumentNullException(orderDetail);

        return new RepositoryOrderDetail(repositoryOrder)
        {
            UnitPrice = orderDetail.UnitPrice,
            Quantity = orderDetail.Quantity,
            Discount = orderDetail.Discount,
            Product = orderDetail.Product.ToRepositoryOrderProduct(),
        };
    }

    public static RepositoryProduct ToRepositoryOrderProduct(this Product product)
    {
        ExceptionHelper.ThrowArgumentNullException(product);

        return new RepositoryProduct(product.ProductID)
        {
            ProductName = product.ProductName,
            SupplierId = product.SupplierID,
            Supplier = product.Supplier.CompanyName,
            CategoryId = product.CategoryID,
            Category = product.Category.CategoryName,
        };
    }

    public static async Task<Order> ToOrderAsync(this RepositoryOrder repoOrder, NorthwindContext context)
    {
        ExceptionHelper.ThrowArgumentNullException(repoOrder);

        var order = new Order
        {
            CustomerID = repoOrder.Customer.Code.Code,
            Customer = await repoOrder.Customer.ToCustomerAsync(context).SetConfigureAwait(),
            EmployeeID = repoOrder.Employee.Id,
            Employee = await repoOrder.Employee.ToEmployeeAsync(context).SetConfigureAwait(),
            OrderDate = repoOrder.OrderDate,
            RequiredDate = repoOrder.RequiredDate,
            ShippedDate = repoOrder.ShippedDate,
            ShipVia = repoOrder.Shipper.Id,
            Shipper = await repoOrder.Shipper.ToShipperAsync(context).SetConfigureAwait(),
            Freight = repoOrder.Freight,
            ShipName = repoOrder.ShipName,
            ShipAddress = repoOrder.ShippingAddress.Address,
            ShipCity = repoOrder.ShippingAddress.City,
            ShipRegion = repoOrder.ShippingAddress.Region,
            ShipPostalCode = repoOrder.ShippingAddress.PostalCode,
            ShipCountry = repoOrder.ShippingAddress.Country,
        };

        foreach (var repoOrderDetail in repoOrder.OrderDetails)
        {
            order.OrderDetails.Add(await repoOrderDetail.ToOrderDetailAsync(order, context).SetConfigureAwait());
        }

        return order;
    }

    public static async Task<Customer> ToCustomerAsync(this RepositoryCustomer repoCustomer, NorthwindContext context)
    {
        ExceptionHelper.ThrowArgumentNullException(repoCustomer);
        ExceptionHelper.ThrowArgumentNullException(context);

        var customer = await context.Customers.FindAsync(repoCustomer.Code.Code).SetConfigureAwait();
        if (customer != null)
        {
            context.Entry(customer).State = EntityState.Unchanged;
            return customer;
        }

        return new Customer
        {
            CustomerID = repoCustomer.Code.Code,
            CompanyName = repoCustomer.CompanyName,
        };
    }

    public static async Task<Employee> ToEmployeeAsync(this RepositoryEmployee repoEmployee, NorthwindContext context)
    {
        ExceptionHelper.ThrowArgumentNullException(repoEmployee);
        ExceptionHelper.ThrowArgumentNullException(context);

        var employee = await context.Employees.FindAsync(repoEmployee.Id).SetConfigureAwait();
        if (employee != null)
        {
            context.Entry(employee).State = EntityState.Unchanged;
            return employee;
        }

        return new Employee
        {
            EmployeeID = repoEmployee.Id,
            FirstName = repoEmployee.FirstName,
            LastName = repoEmployee.LastName,
            Country = repoEmployee.Country,
        };
    }

    public static async Task<Shipper> ToShipperAsync(this RepositoryShipper repoShipper, NorthwindContext context)
    {
        ExceptionHelper.ThrowArgumentNullException(repoShipper);
        ExceptionHelper.ThrowArgumentNullException(context);

        var shipper = await context.Shippers.FindAsync(repoShipper.Id).SetConfigureAwait();
        if (shipper != null)
        {
            context.Entry(shipper).State = EntityState.Unchanged;
            return shipper;
        }

        return new Shipper
        {
            ShipperID = repoShipper.Id,
            CompanyName = repoShipper.CompanyName,
        };
    }

    public static async Task<OrderDetail> ToOrderDetailAsync(this RepositoryOrderDetail repoOrderDetail, Order order, NorthwindContext context)
    {
        ExceptionHelper.ThrowArgumentNullException(repoOrderDetail);
        ExceptionHelper.ThrowArgumentNullException(order);

        var product = await repoOrderDetail.Product.ToProductAsync(context).SetConfigureAwait();

        return new OrderDetail
        {
            OrderID = order.OrderID,
            Order = order,
            ProductID = product.ProductID,
            Product = product,
            UnitPrice = repoOrderDetail.UnitPrice,
            Discount = repoOrderDetail.Discount,
            Quantity = repoOrderDetail.Quantity,
        };
    }

    public static async Task<Product> ToProductAsync(this RepositoryProduct repoProduct, NorthwindContext context)
    {
        ExceptionHelper.ThrowArgumentNullException(repoProduct);
        ExceptionHelper.ThrowArgumentNullException(context);

        var product = await context.Products.FindAsync(repoProduct.Id).SetConfigureAwait();
        if (product != null)
        {
            context.Entry(product).State = EntityState.Unchanged;
            return product;
        }

        var category = await context.Categories.FindAsync(repoProduct.CategoryId).SetConfigureAwait();
        if (category != null)
        {
            context.Entry(category).State = EntityState.Unchanged;
        }
        else
        {
            category = new Category() { CategoryID = repoProduct.CategoryId, CategoryName = repoProduct.Category };
        }

        var supplier = await context.Suppliers.FindAsync(repoProduct.SupplierId).SetConfigureAwait();
        if (supplier != null)
        {
            context.Entry(supplier).State = EntityState.Unchanged;
        }
        else
        {
            supplier = new Supplier() { SupplierID = repoProduct.SupplierId, CompanyName = repoProduct.Supplier };
        }

        return new Product
        {
            ProductID = repoProduct.Id,
            ProductName = repoProduct.ProductName,
            SupplierID = repoProduct.SupplierId,
            Supplier = supplier,
            CategoryID = repoProduct.CategoryId,
            Category = category,
        };
    }
}
