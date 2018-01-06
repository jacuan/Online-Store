using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DigitalXData;

namespace WCF
{
    public class OrderInformationService : IOrderInformationService
    {
        DigitalXDB_JackyWebEntities db = new DigitalXDB_JackyWebEntities();

        public List<OrderedItem> GetOrderList(int id)
        {
            List<Order> orderList = (from order in db.Orders
                                     where order.CustomerID == id
                                     select order).ToList();

            if (orderList.Count != 0)
            {
                var orderInformation = (from o in db.Orders
                                        where o.CustomerID == id
                                        join od in db.OrderDetails on o.OrderID equals od.OrderID
                                        join p in db.Products on od.ProductID equals p.ProductID
                                        select new
                                        {
                                            OrderId = o.OrderID,
                                            OrderDate = o.OrderDate,
                                            Complete = o.Complete,
                                            IsBackOrder = o.IsBackOrder,
                                            Quantity = od.Quantity,
                                            ProductName = p.ProductName,
                                            Picture = p.Picture,
                                            Price = p.Price
                                        }).ToList().Select(x => new OrderedItem()
                                        {
                                            Product = new Product()
                                            {
                                                ProductName = x.ProductName,
                                                Picture = x.Picture,
                                                Price = x.Price
                                            },
                                            OrderId = x.OrderId,
                                            OrderDate = x.OrderDate,
                                            Complete = x.Complete,
                                            IsBackOrder = x.IsBackOrder,
                                            Quantity = x.Quantity
                                        });

                return orderInformation.ToList();
            }
            else
            {
                throw new FaultException<RequestFailed>(
                    new RequestFailed()
                    {
                        Message = "Order information cannot be found becuase you have not placed any order yet.",
                        ErrorCode = 404
                    }, "Invalid Request.");
            }  
        }

        public List<ShoppingCartInformation> GetShoppingCartInformation(int id)
        {
            var query = (from q in db.Orders
                         where q.CustomerID == id &&
                         q.IsBackOrder == false &&
                         q.Complete == false
                         select q).ToList();

            if (query.Count() != 0)
            {
                var customer = (from c in db.Customers
                                where c.CustomerID == id
                                select c).FirstOrDefault();

                if (customer != null)
                {
                    var itemsInShoppingCart = (from o in db.Orders
                                               where o.CustomerID == id && o.Complete == false && o.IsBackOrder == false
                                               join c in db.Customers on o.CustomerID equals c.CustomerID
                                               join od in db.OrderDetails on o.OrderID equals od.OrderID
                                               join p in db.Products on od.ProductID equals p.ProductID
                                               select new
                                               {
                                                   ProductId = p.ProductID,
                                                   ProductName = p.ProductName,
                                                   Quantity = od.Quantity,
                                                   Price = p.Price,
                                                   OrderDetailId = od.DetailID,
                                                   OrderId = o.OrderID,
                                                   Addresses = c.Addresses
                                               }).ToArray().Select(x => new ShoppingCartInformation()
                                               {
                                                   OrderDetailId = x.OrderDetailId,
                                                   Price = x.Price,
                                                   ProductId = x.ProductId,
                                                   ProductName = x.ProductName,
                                                   Quantity = x.Quantity,
                                                   OrderId = x.OrderId,
                                                   TheAddresses = x.Addresses.ToList()
                                               });

                    return itemsInShoppingCart.ToList();
                }
                else
                {
                    throw new FaultException<RequestFailed>(
                    new RequestFailed()
                    {
                        Message = "An error occured while processing your request. Please try again later.",
                        ErrorCode = 500
                    }, "Unknow Error.");
                }
            }
            else
            {
                throw new FaultException<RequestFailed>(
                    new RequestFailed()
                    {
                        Message = "Your shopping cart is empty, please add an item to shopping cart first.",
                        ErrorCode = 404
                    }, "Invalid Request.");
            }
        }

    }
}
