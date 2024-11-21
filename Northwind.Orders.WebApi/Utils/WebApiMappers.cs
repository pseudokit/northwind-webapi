using ModelsBriefOrder = Northwind.Orders.WebApi.Models.BriefOrder;
using RepositoryOrder = Northwind.Services.Repositories.Order;
using RepositoryShipper = Northwind.Services.Repositories.Shipper;
using RepositoryShippingAddres = Northwind.Services.Repositories.ShippingAddress;

namespace Northwind.Orders.WebApi.Utils;

public static class WebApiMappers
{
    public static RepositoryOrder ModelsBriefOrderMapper(this ModelsBriefOrder order, long orderId)
    {
        var newOrder = new RepositoryOrder(orderId)
        {
            OrderDate = order.OrderDate,

            RequiredDate = order.RequiredDate,

            ShippedDate = order.ShippedDate,

            Shipper = new RepositoryShipper(order.ShipperId)
            {
                CompanyName = order.ShipName,
            },

            Freight = order.Freight,

            ShipName = order.ShipName,

            ShippingAddress = new RepositoryShippingAddres(
                    order.ShipAddress,
                    order.ShipCity,
                    order.ShipRegion,
                    order.ShipPostalCode,
                    order.ShipCountry),
        };
        return newOrder;
    }
}
