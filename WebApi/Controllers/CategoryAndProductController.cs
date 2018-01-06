using DigitalXData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    [Authorize]
    public class CategoryAndProductController : ApiController
    {
        DigitalXDB_JackyWebEntities db = new DigitalXDB_JackyWebEntities();

        // GET: api/CategoryAndProduct

        [AllowAnonymous]
        [HttpGet]
        [Route("api/CategoryAndProduct/GetCategoryList")]
        public List<ProductCategory> GetCategoryList()
        {
                return db.ProductCategories.ToList();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/CategoryAndProduct/GetPopularProductList")]
        public List<PopularProduct> GetPopularProductList()
        {
            var queries = (from order in db.OrderDetails
                          group order by order.ProductID into grouping
                          select new
                          {
                              id = grouping.Key,
                              sum = grouping.Sum(q => q.Quantity)
                          }).OrderByDescending(s => s.sum).Take(5).ToList();

            var popularProducts = (from query in queries
                                   join item in db.Products
                                   on query.id equals item.ProductID
                                   select new PopularProduct()
                                   {
                                       ProductName = item.ProductName,
                                       ProductDescription = item.ProductDescription,
                                       UnitsInStock = item.UnitsInStock,
                                       Price = item.Price,
                                       Picture = item.Picture,
                                       ProductID = item.ProductID
                                   }).ToList();
            return popularProducts;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/CategoryAndProduct/GetProductList/{id}")]
        public List<ProductDTO> GetProductList(int id)
        {
            List<ProductDTO> products = null;

            var parentCategory = (from parentCat in db.ProductCategories
                                  where parentCat.ParentCategory == 0
                                  && parentCat.CategoryID == id
                                  select parentCat).FirstOrDefault();

            if (parentCategory != null)
            {
                var subCategories = (from subCategory in db.ProductCategories
                                     where subCategory.ParentCategory != 0
                                     select subCategory).ToList();

                var queries = (from query in subCategories
                               where query.ParentCategory == parentCategory.CategoryID
                               select query).ToList();

                products = (from item in queries
                            join product in db.Products
                            on item.CategoryID equals product.CategoryID
                            select new ProductDTO()
                            {
                                ProductName = product.ProductName,
                                ProductDescription = product.ProductDescription,
                                UnitsInStock = product.UnitsInStock,
                                Price = product.Price,
                                Picture = product.Picture,
                                ProductId = product.ProductID
                                
                            }).ToList();
                if (products != null)
                {
                    return products;
                }
                return null;
            }
            else
            {
                products = (from product in db.Products
                            where product.CategoryID == id
                            select new ProductDTO()
                            {
                                ProductName = product.ProductName,
                                ProductDescription = product.ProductDescription,
                                UnitsInStock = product.UnitsInStock,
                                Price = product.Price,
                                Picture = product.Picture,
                                ProductId = product.ProductID
                            }).ToList();

                if (products != null)
                {
                    return products;
                }
                return null;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/CategoryAndProduct/GetProductDetail/{id}")]
        public ProductDTO GetProductDetail(int id)
        {
            ProductDTO productDTO = (from item in db.Products
                            where item.ProductID == id
                            select new ProductDTO()
                            {
                                ProductName = item.ProductName,
                                ProductDescription = item.ProductDescription,
                                UnitsInStock = item.UnitsInStock,
                                Price = item.Price,
                                Picture = item.Picture,
                                ProductId = item.ProductID
                            }).FirstOrDefault();

            if (productDTO != null)
            {
                return productDTO;
            }
            return null;
        }
    }
}
