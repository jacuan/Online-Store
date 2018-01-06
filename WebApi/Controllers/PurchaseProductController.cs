using DigitalXData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebApi.Controllers
{
    [Authorize]
    public class PurchaseProductController : ApiController
    {
        DigitalXDB_JackyWebEntities db = new DigitalXDB_JackyWebEntities();

        [HttpPost]
        [Route("api/PurchaseProduct/AddtoShoppingCart/{id}")]
        public async Task<AddToShoppingCart> AddtoShoppingCart(int id, AddToShoppingCart addToShoppingCart)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var customer = await (from c in db.Customers
                                    where c.CustomerID == id
                                    select c).FirstOrDefaultAsync();

                    if (customer!= null && customer.Addresses.Count == 0)
                    {
                        Address address = new Address();
                        address.AddressType = 1;
                        address.Street = "";
                        address.Suburb = "";
                        address.City = "";
                        address.Country = "";
                        address.PostalCode = "Dummy";
                        customer.Addresses.Add(address);
                        await db.SaveChangesAsync();

                        var query = await (from o in db.Orders
                                           where o.CustomerID == id
                                           && o.Complete == false
                                           && o.IsBackOrder == false
                                           select o).FirstOrDefaultAsync();

                        if (query == null)
                        {
                            Order order = new Order();
                            order.AddressID = address.AddressID;
                            order.CustomerID = id;
                            order.OrderDate = DateTime.Now;
                            order.Complete = false;
                            order.IsBackOrder = false;
                            db.Orders.Add(order);
                            customer.Orders.Add(order);
                            await db.SaveChangesAsync();

                            OrderDetail orderDetail = new OrderDetail();
                            orderDetail.OrderID = order.OrderID;
                            orderDetail.ProductID = addToShoppingCart.ProductId;
                            orderDetail.Quantity = addToShoppingCart.Quantity;
                            orderDetail.Packaged = false;
                            db.OrderDetails.Add(orderDetail);
                            order.OrderDetails.Add(orderDetail);
                            await db.SaveChangesAsync();

                            return addToShoppingCart;
                        }
                        else
                        {
                            var orderDetail = await (from od in db.OrderDetails
                                               where od.OrderID == query.OrderID &&
                                               od.ProductID == addToShoppingCart.ProductId
                                               select od).FirstOrDefaultAsync();

                            if (orderDetail == null)
                            {
                                OrderDetail newDetail = new OrderDetail();
                                newDetail.OrderID = query.OrderID;
                                newDetail.ProductID = addToShoppingCart.ProductId;
                                newDetail.Quantity = addToShoppingCart.Quantity;
                                newDetail.Packaged = false;
                                db.OrderDetails.Add(newDetail);
                                query.OrderDetails.Add(newDetail);

                                await db.SaveChangesAsync();

                                return addToShoppingCart;
                            }
                            else
                            {
                                orderDetail.Quantity = orderDetail.Quantity  + addToShoppingCart.Quantity;
                                await db.SaveChangesAsync();
                                return addToShoppingCart;
                            }
                        } 
                    }
                    else if (customer != null && customer.Addresses.Count != 0)
                    {
                        var query = await (from o in db.Orders
                                           where o.CustomerID == id
                                           && o.Complete == false
                                           && o.IsBackOrder == false
                                           select o).FirstOrDefaultAsync();

                        if (query == null)
                        {
                            var address = (from a in customer.Addresses
                                                 select a).FirstOrDefault();

                            Order order = new Order();
                            order.AddressID = address.AddressID;
                            order.CustomerID = id;
                            order.OrderDate = DateTime.Now;
                            order.Complete = false;
                            order.IsBackOrder = false;
                            db.Orders.Add(order);
                            customer.Orders.Add(order);
                            await db.SaveChangesAsync();

                            OrderDetail orderDetail = new OrderDetail();
                            orderDetail.OrderID = order.OrderID;
                            orderDetail.ProductID = addToShoppingCart.ProductId;
                            orderDetail.Quantity = addToShoppingCart.Quantity;
                            orderDetail.Packaged = false;
                            db.OrderDetails.Add(orderDetail);
                            order.OrderDetails.Add(orderDetail);

                            await db.SaveChangesAsync();

                            return addToShoppingCart;
                        }
                        else
                        {
                            var orderDetail = await (from od in db.OrderDetails
                                                     where od.OrderID == query.OrderID &&
                                                     od.ProductID == addToShoppingCart.ProductId
                                                     select od).FirstOrDefaultAsync();

                            if (orderDetail == null)
                            {
                                OrderDetail newDetail = new OrderDetail();
                                newDetail.OrderID = query.OrderID;
                                newDetail.ProductID = addToShoppingCart.ProductId;
                                newDetail.Quantity = addToShoppingCart.Quantity;
                                newDetail.Packaged = false;
                                db.OrderDetails.Add(newDetail);
                                query.OrderDetails.Add(newDetail);

                                await db.SaveChangesAsync();

                                return addToShoppingCart;
                            }
                            else
                            {
                                orderDetail.Quantity = orderDetail.Quantity + addToShoppingCart.Quantity;
                                await db.SaveChangesAsync();
                                return addToShoppingCart;
                            }
                        }
                    }
                    return null;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("api/PurchaseProduct/CountShoppingCartItems/{id}")]
        public int CountShoppingCartItems(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var queries = (from order in db.Orders
                                   where order.CustomerID == id && order.Complete == false && order.IsBackOrder == false
                                   join orderDetail in db.OrderDetails on order.OrderID equals orderDetail.OrderID
                                   select orderDetail);

                    return queries.Count();
                }
                return 0;
            }
            catch(Exception)
            {
                return 0;
            }
        }

        [HttpPost]
        [Route("api/PurchaseProduct/AdjustQuantity/{id}")]
        public async Task<AdjustQuantity> AdjustQuantity(int id, AdjustQuantity quantity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var query = await (from q in db.OrderDetails
                                       where q.DetailID == id
                                       select q).FirstOrDefaultAsync();
                    if (query != null)
                    {
                        query.Quantity = quantity.AdjustedQuantity;
                        db.Entry(query).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        return quantity;
                    }
                    return null;
                }
                return null;
            }   
            catch (Exception)
            {
                return null;
            }
        }

        [HttpGet]
        [Route("api/PurchaseProduct/DeleteItem/{id}")]
        public async Task<int> DeleteItem(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var query = await (from q in db.OrderDetails
                                       where q.DetailID == id
                                       select q).FirstOrDefaultAsync();

                    if (query != null)
                    {
                        db.OrderDetails.Remove(query);
                        await db.SaveChangesAsync();
                        return 1;
                    }
                    return 0;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        [HttpGet]
        [Route("api/PurchaseProduct/VerifyStock/{id}")]
        public List<VerifiyStock> VerifyStock(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var listOfVerfityStock = (from o in db.Orders
                                              where o.OrderID == id && o.Complete == false && o.IsBackOrder == false
                                              join od in db.OrderDetails on o.OrderID equals od.OrderID
                                              join p in db.Products on od.ProductID equals p.ProductID
                                              select new
                                              {
                                                  orderedQuantity = od.Quantity,
                                                  availableStock = p.UnitsInStock,
                                                  productName = p.ProductName
                                              }).ToArray()
                               .Select(x => new VerifiyStock()
                               {
                                   OrderedQuantity = x.orderedQuantity,
                                   AvailableStock = x.availableStock,
                                   ProductName = x.productName
                               }).ToList();

                    return listOfVerfityStock;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }


        [HttpGet]
        [Route("api/PurchaseProduct/DeductStockConditionaly/{id}")]
        public async Task<int> DeductStockConditionaly(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Order order = await db.Orders.FindAsync(id);

                    if (order.Complete == false)
                    {
                        order.Complete = true;
                        order.IsBackOrder = false;
                        await db.SaveChangesAsync();

                        var orderDetails = (from od in db.OrderDetails
                                            where od.OrderID == order.OrderID
                                            select od).ToList();

                        foreach (var item in orderDetails)
                        {
                            var product = await (from p in db.Products
                                                 where p.ProductID == item.ProductID
                                                 select p).FirstOrDefaultAsync();

                            product.UnitsInStock = product.UnitsInStock - item.Quantity; 
                        }
                        await db.SaveChangesAsync();
                        return 1;
                    }
                    return 0;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        [HttpGet]
        [Route("api/PurchaseProduct/DeductStockWithBackorder/{id}")]
        public async Task<int> DeductStockWithBackorder(int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Order order = await db.Orders.FindAsync(id);

                    if (order !=null && order.Complete == false)
                    {
                        var purchasedProducts = (from od in db.OrderDetails
                                        where od.OrderID == order.OrderID
                                        join p in db.Products on od.ProductID equals p.ProductID
                                        select new
                                        {
                                            orderedQuantity = od.Quantity,
                                            availableStock = p.UnitsInStock,
                                            productId = p.ProductID
                                        }).ToList();

                        if (purchasedProducts != null)
                        {
                            foreach (var item in purchasedProducts)
                            {
                                if (item.availableStock >= item.orderedQuantity)
                                {
                                    var product = await (from p in db.Products
                                                         where p.ProductID == item.productId
                                                         select p).FirstOrDefaultAsync();

                                    product.UnitsInStock = product.UnitsInStock - item.orderedQuantity;                                                                    
                                }
                                else if (item.availableStock < item.orderedQuantity)
                                {
                                    var product = await (from p in db.Products
                                                         where p.ProductID == item.productId
                                                         select p).FirstOrDefaultAsync();

                                    product.UnitsInStock = 0;
                                    
                                    var orderDetail = await (from od in db.OrderDetails
                                                             where od.OrderID == order.OrderID && od.ProductID == item.productId
                                                             select od).FirstOrDefaultAsync();

                                        if (item.availableStock != 0)
                                        {
                                            orderDetail.Quantity = item.availableStock;
                                        }
                                        else if (item.availableStock == 0)
                                        {
                                            order.OrderDetails.Remove(orderDetail);
                                            db.OrderDetails.Remove(orderDetail);
                                        }
                                }
                            }
                                order.Complete = true;
                                order.IsBackOrder = false;
                                await db.SaveChangesAsync();

                            var customer = await (from c in db.Customers
                                                  where c.CustomerID == order.CustomerID
                                                  select c).FirstOrDefaultAsync();

                            if (customer != null)
                            {
                                Order backorder = new Order();
                                backorder.AddressID = order.AddressID;
                                backorder.CustomerID = order.CustomerID;
                                backorder.OrderDate = DateTime.Now;
                                backorder.Complete = false;
                                backorder.IsBackOrder = true;
                                db.Orders.Add(backorder);
                                customer.Orders.Add(backorder);
                                await db.SaveChangesAsync();

                                foreach (var item in purchasedProducts)
                                {
                                    if (item.availableStock < item.orderedQuantity)
                                    {
                                        OrderDetail backorderDetail = new OrderDetail();
                                        backorderDetail.OrderID = backorder.OrderID;
                                        backorderDetail.ProductID = item.productId;
                                        backorderDetail.Quantity = item.orderedQuantity - item.availableStock;
                                        backorderDetail.Packaged = false;
                                        db.OrderDetails.Add(backorderDetail);
                                        backorder.OrderDetails.Add(backorderDetail);
                                        await db.SaveChangesAsync();
                                    }
                                }   
                            }
                            else
                            {
                                return 0;
                            }
                            return 1;
                        }
                        return 0;
                    }
                    return 0;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
