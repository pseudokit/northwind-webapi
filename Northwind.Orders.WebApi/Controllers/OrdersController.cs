using Microsoft.AspNetCore.Mvc;
using Northwind.Orders.WebApi.Utils;
using Northwind.Services.Repositories;

using ModelsAddOrder = Northwind.Orders.WebApi.Models.AddOrder;
using ModelsBriefOrder = Northwind.Orders.WebApi.Models.BriefOrder;
using ModelsBriefOrderDetail = Northwind.Orders.WebApi.Models.BriefOrderDetail;
using ModelsCustomer = Northwind.Orders.WebApi.Models.Customer;
using ModelsEmployee = Northwind.Orders.WebApi.Models.Employee;
using ModelsFullOrder = Northwind.Orders.WebApi.Models.FullOrder;
using ModelsShipper = Northwind.Orders.WebApi.Models.Shipper;
using ModelsShippingAddress = Northwind.Orders.WebApi.Models.ShippingAddress;

using RepositoryOrder = Northwind.Services.Repositories.Order;
namespace Northwind.Orders.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderRepository repository;
    private readonly ILogger<OrdersController> logger;

    public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
    {
        this.repository = orderRepository;
        this.logger = logger;
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<ModelsFullOrder>> GetOrderAsync(long orderId)
    {
        this.logger.LogInformation("get order by orderId", orderId);
        RepositoryOrder order;
        try
        {
            order = await this.repository.GetOrderAsync(orderId);
        }
        catch (OrderNotFoundException)
        {
            return this.NotFound();
        }
        catch (Exception)
        {
            return this.StatusCode(500);
        }

        var result = new ModelsFullOrder()
        {
            Id = order.Id,

            Customer = new ModelsCustomer()
            {
                Code = order.Customer.Code.Code,
                CompanyName = order.Customer.CompanyName,
            },

            Employee = new ModelsEmployee()
            {
                Id = order.Employee.Id,
                FirstName = order.Employee.FirstName,
                LastName = order.Employee.LastName,
                Country = order.Employee.Country,
            },

            OrderDate = order.OrderDate,

            RequiredDate = order.RequiredDate,

            ShippedDate = order.ShippedDate,

            Shipper = new ModelsShipper
            {
                Id = order.Shipper.Id,
                CompanyName = order.Shipper.CompanyName,
            },

            Freight = order.Freight,

            ShipName = order.ShipName,

            ShippingAddress = new ModelsShippingAddress
            {
                Address = order.ShippingAddress.Address,
                City = order.ShippingAddress.City,
                Region = order.ShippingAddress.Region,
                PostalCode = order.ShippingAddress.PostalCode,
                Country = order.ShippingAddress.Country,
            },
        };

        return this.Ok(result);
    }

    [HttpGet("{skip}/{count}")]
    public async Task<ActionResult<IEnumerable<ModelsBriefOrder>>> GetOrdersAsync(int? skip, int? count)
    {
        skip = skip == null ? 0 : skip;
        count = count == null ? 10 : count;

        if (skip < 0 || count < 1)
        {
            return this.BadRequest();
        }

        this.logger.LogInformation("get orders by skip count", skip!);
        IList<RepositoryOrder>? list;
        try
        {
            list = await this.repository.GetOrdersAsync((int)skip, (int)count!);
        }
        catch (Exception)
        {
            return this.StatusCode(500);
        }

        var result = new List<ModelsBriefOrder>();

        foreach (var or in list)
        {
            var briefOrderDetails = new List<ModelsBriefOrderDetail>();
            foreach (var od in or.OrderDetails)
            {
                briefOrderDetails.Add(new ModelsBriefOrderDetail()
                {
                    ProductId = od.Product.Id,
                    UnitPrice = od.UnitPrice,
                    Quantity = od.Quantity,
                    Discount = od.Discount,
                });
            }

            var br = new ModelsBriefOrder()
            {
                Id = or.Id,

                CustomerId = or.Customer.Code.Code,

                EmployeeId = or.Employee.Id,

                OrderDate = or.OrderDate,

                RequiredDate = or.RequiredDate,

                ShippedDate = or.ShippedDate,

                ShipperId = or.Shipper.Id,

                Freight = or.Freight,

                ShipName = or.ShipName,

                ShipAddress = or.ShippingAddress.Address,

                ShipCity = or.ShippingAddress.City,

                ShipRegion = or.ShippingAddress.Region,

                ShipPostalCode = or.ShippingAddress.PostalCode,

                ShipCountry = or.ShippingAddress.Country,

                OrderDetails = briefOrderDetails,
            };

            result.Add(br);
        }

        return this.Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ModelsAddOrder>> AddOrderAsync(ModelsBriefOrder order)
    {
        this.logger.LogInformation("add order", order);
        var newOrder = order.ModelsBriefOrderMapper(order.Id);

        long newOrderId;
        try
        {
            newOrderId = await this.repository.AddOrderAsync(newOrder);
        }
        catch (Exception)
        {
            return this.StatusCode(500);
        }

        return this.Ok(new ModelsAddOrder() { OrderId = newOrderId });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> RemoveOrderAsync(long orderId)
    {
        this.logger.LogInformation("remove order by id", orderId);
        try
        {
            await this.repository.RemoveOrderAsync(orderId);
        }
        catch (OrderNotFoundException)
        {
            return this.NotFound();
        }
        catch (Exception)
        {
            return this.StatusCode(500);
        }

        return this.NoContent();
    }

    [HttpPut("{orderId}")]
    public async Task<ActionResult> UpdateOrderAsync(long orderId, ModelsBriefOrder order)
    {
        this.logger.LogInformation("update order by id", orderId);
        try
        {
            var newOrder = order.ModelsBriefOrderMapper(orderId);

            await this.repository.UpdateOrderAsync(newOrder);
        }
        catch (OrderNotFoundException)
        {
            return this.NotFound();
        }
        catch (Exception)
        {
            return this.StatusCode(500);
        }

        return this.NoContent();
    }
}
