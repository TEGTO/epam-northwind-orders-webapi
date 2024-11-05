using Northwind.Orders.WebApi.Models;
using Northwind.Services.Repositories;
using Customer = Northwind.Orders.WebApi.Models.Customer;
using Employee = Northwind.Orders.WebApi.Models.Employee;
using RepositoryCustomer = Northwind.Services.Repositories.Customer;
using RepositoryEmployee = Northwind.Services.Repositories.Employee;
using RepositoryOrder = Northwind.Services.Repositories.Order;
using RepositoryOrderDetail = Northwind.Services.Repositories.OrderDetail;
using RepositoryShipper = Northwind.Services.Repositories.Shipper;
using RepositoryShippingAddress = Northwind.Services.Repositories.ShippingAddress;
using Shipper = Northwind.Orders.WebApi.Models.Shipper;
using ShippingAddress = Northwind.Orders.WebApi.Models.ShippingAddress;

namespace Northwind.Orders.WebApi;

public static class OrderControllerMappingExtensions
{
    public static BriefOrder ToBriefOrder(this RepositoryOrder order)
    {
        ExceptionHelper.ThrowArgumentNullException(order);

        var briefOrder = new BriefOrder
        {
            Id = order.Id,
            CustomerId = order.Customer.Code.Code,
            EmployeeId = order.Employee.Id,
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            ShipperId = order.Shipper.Id,
            Freight = order.Freight,
            ShipName = order.ShipName,
            ShipAddress = order.ShippingAddress.Address,
            ShipCity = order.ShippingAddress.City,
            ShipRegion = order.ShippingAddress.Region,
            ShipPostalCode = order.ShippingAddress.PostalCode,
            ShipCountry = order.ShippingAddress.Country,
        };

        foreach (var detail in order.OrderDetails)
        {
            briefOrder.OrderDetails.Add(detail.ToBriefOrderDetail());
        }

        return briefOrder;
    }

    public static BriefOrderDetail ToBriefOrderDetail(this RepositoryOrderDetail orderDetail)
    {
        ExceptionHelper.ThrowArgumentNullException(orderDetail);

        return new BriefOrderDetail
        {
            ProductId = orderDetail.Product.Id,
            UnitPrice = orderDetail.UnitPrice,
            Quantity = orderDetail.Quantity,
            Discount = orderDetail.Discount,
        };
    }

    public static RepositoryOrder ToOrder(this BriefOrder briefOrder)
    {
        ExceptionHelper.ThrowArgumentNullException(briefOrder);

        var order = new RepositoryOrder(briefOrder.Id)
        {
            Customer = new RepositoryCustomer(new CustomerCode(briefOrder.CustomerId)),
            Employee = new RepositoryEmployee(briefOrder.EmployeeId),
            OrderDate = briefOrder.OrderDate,
            RequiredDate = briefOrder.RequiredDate,
            ShippedDate = briefOrder.ShippedDate,
            Shipper = new RepositoryShipper(briefOrder.ShipperId),
            Freight = briefOrder.Freight,
            ShipName = briefOrder.ShipName,
            ShippingAddress = new RepositoryShippingAddress(briefOrder.ShipAddress, briefOrder.ShipCity, briefOrder.ShipRegion, briefOrder.ShipPostalCode, briefOrder.ShipCountry),
        };

        foreach (var briefDetails in briefOrder.OrderDetails)
        {
            order.OrderDetails.Add(briefDetails.ToOrderDetail(order));
        }

        return order;
    }

    public static RepositoryOrderDetail ToOrderDetail(this BriefOrderDetail briefOrderDetail, RepositoryOrder order)
    {
        ExceptionHelper.ThrowArgumentNullException(briefOrderDetail);

        return new RepositoryOrderDetail(order)
        {
            Product = new Product(briefOrderDetail.ProductId),
            UnitPrice = briefOrderDetail.UnitPrice,
            Quantity = briefOrderDetail.Quantity,
            Discount = briefOrderDetail.Discount,
        };
    }

    public static FullOrder ToFullOrder(this RepositoryOrder order)
    {
        ExceptionHelper.ThrowArgumentNullException(order);

        var fullOrder = new FullOrder
        {
            Id = order.Id,
            Customer = order.Customer.ToCustomerModel(),
            Employee = order.Employee.ToEmployeeModel(),
            OrderDate = order.OrderDate,
            RequiredDate = order.RequiredDate,
            ShippedDate = order.ShippedDate,
            Shipper = order.Shipper.ToShipperModel(),
            Freight = order.Freight,
            ShipName = order.ShipName,
            ShippingAddress = order.ShippingAddress.ToShippingAddressModel(),
        };

        foreach (var detail in order.OrderDetails)
        {
            fullOrder.OrderDetails.Add(detail.ToFullOrderDetail());
        }

        return fullOrder;
    }

    public static FullOrderDetail ToFullOrderDetail(this RepositoryOrderDetail orderDetail)
    {
        ExceptionHelper.ThrowArgumentNullException(orderDetail);

        return new FullOrderDetail
        {
            ProductId = orderDetail.Product.Id,
            ProductName = orderDetail.Product.ProductName,
            CategoryId = orderDetail.Product.Id,
            CategoryName = orderDetail.Product.Category,
            SupplierId = orderDetail.Product.Id,
            SupplierCompanyName = orderDetail.Product.Supplier,
            UnitPrice = orderDetail.UnitPrice,
            Quantity = orderDetail.Quantity,
            Discount = orderDetail.Discount,
        };
    }

    public static Customer ToCustomerModel(this RepositoryCustomer customer)
    {
        ExceptionHelper.ThrowArgumentNullException(customer);

        return new Customer
        {
            Code = customer.Code.Code,
            CompanyName = customer.CompanyName,
        };
    }

    public static Employee ToEmployeeModel(this RepositoryEmployee employee)
    {
        ExceptionHelper.ThrowArgumentNullException(employee);

        return new Employee
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Country = employee.Country,
        };
    }

    public static Shipper ToShipperModel(this RepositoryShipper shipper)
    {
        ExceptionHelper.ThrowArgumentNullException(shipper);

        return new Shipper
        {
            Id = shipper.Id,
            CompanyName = shipper.CompanyName,
        };
    }

    public static ShippingAddress ToShippingAddressModel(this RepositoryShippingAddress shippingAddress)
    {
        ExceptionHelper.ThrowArgumentNullException(shippingAddress);

        return new ShippingAddress
        {
            Address = shippingAddress.Address,
            City = shippingAddress.City,
            Region = shippingAddress.Region,
            PostalCode = shippingAddress.PostalCode,
            Country = shippingAddress.Country,
        };
    }
}
