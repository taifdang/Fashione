using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using eCommerce_API.Models;
using eCommerce_API.DTO;
using eCommerce_API.Service;
using AutoMapper;
using System.ComponentModel;
using Microsoft.CodeAnalysis;

namespace eCommerce_API.Controllers
{
    [Route("api/categorytypes")]
    [ApiController]
    public class CategoryTypesController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ImageService _imageService;


        public CategoryTypesController(DatabaseContext context, ImageService imageService)
        {
            _databaseContext = context;
            _imageService = imageService;
        }

        // GET: api/CategoryTypes
        [HttpGet]
        public async Task<ActionResult> GetCategoriesType()
        {
            if (_databaseContext.CategoriesType == null)
            {
                return NotFound();
            }
            //var data = await _databaseContext.CategoriesType
            //      .Include(c => c.category)
            //      .Select(x => new
            //      {
            //          x.id,
            //          x.name,
            //          category = x.category.Select(y => new CategoryDTO
            //          {
            //              id = y.id,
            //              name = y.name,
            //              products = y.products.Select(p => new ProductDTO
            //              {
            //                  id = p.id,
            //                  name = p.name,
            //                  price = p.price,
            //                  sale = p.sale,
            //                  description = p.description,
            //                  image = _imageService.GetImageUrl(p.image),
            //                  category = p.category.name,
            //                  categoryId = p.categoryId,
            //              }).ToList()

            //          }).ToList()
            //      }).ToListAsync();
            //var product = await _databaseContext.CategoriesType.SelectMany(c=>c.category).SelectMany(p=>p.products).ToListAsync();
            var data = await _databaseContext.CategoriesType
                .Include(x => x.category)
                .ThenInclude(y => y.products)
                .Select(z => new CategoryTypeDTO
                {
                    id = z.id,
                    name = z.name,
                    categories = z.category.Select(c => new CategoryDTO
                    {
                        id =c.id,
                        name =c.name,
                        products = c.products.Select(p=>new ProductDTO
                        {
                            id = p.id,
                            name = p.name,
                            price = p.price,
                            sale = p.sale,
                            description = p.description,
                            image = _imageService.GetImageUrl( p.image),
                            category = p.category.name,
                            categoryId = p.categoryId,
                        })
                        .ToList()
                    }).ToList(),
                }).ToListAsync();
            return Ok(data);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetId(int id)
        {
            //SelectMany
            //var hasID = await _databaseContext.CategoriesType.FindAsync(id);
            //if (hasID == null) return NotFound();
            //var product = await _databaseContext.CategoriesType
            //    .Include(x => x.category)
            //    .ThenInclude(y => y.products)
            //    .Where(x => x.id == id)
            //    .Select(y => new CategoryTypeDTO
            //    {
            //        id = y.id,
            //        name = y.name,
            //        categories = y.category.Select(c => new CategoryDTO {
            //            id = c.id,
            //            name = c.name,
            //            products = c.products.Select(p=> new ProductDTO
            //            {
            //                id=p.id,
            //                name = p.name,
            //                price = p.price,
            //                sale = p.sale,
            //                description = p.description,
            //                image = _imageService.GetImageUrl(p.image),
            //                category = p.category.name,
            //                categoryId = c.id,

            //            }).ToList()

            //        }).ToList(),


            //    }).ToListAsync();
            var hasID = await _databaseContext.CategoriesType
                  .Include(x => x.category).ThenInclude(y => y.products).FirstOrDefaultAsync(p => p.id == id);
            //
            if (hasID == null) return Ok(new Response() { statusCode = 404, message = "not found data", data = null });
            //
            var products = hasID.category
           .Select(c=>new CategoryDTO
           {
               id=c.id,
               name =c.name,
               products = c.products.Select(p=> new ProductDTO
               {
                   id = p.id,
                   name = p.name,
                   price = p.price,
                   sale = p.sale,
                   description = p.description,
                   image = _imageService.GetImageUrl(p.image), 
                   category = p.category.name,
                   categoryId = p.categoryId,

               }).ToList(),
           }).ToList();
           
            return Ok(products);

        }
        [HttpGet("sort")]
        public async Task<ActionResult> sort2DTO(int? typeId, int? categoryID, SortType sorttype, int currentPage, int pageSize)
        {
            var response = new Response();
            try
            {
                IEnumerable<ProductDTO?> products;
                if (typeId == null)  products = await _databaseContext.CategoriesType
                    .Include(x => x.category)
                    .ThenInclude(y => y.products)
                    .SelectMany(c => c.category)
                    .SelectMany(p => p.products)
                    .Select(x => new ProductDTO
                    {
                        id = x.id,
                        name = x.name,
                        price = x.price,
                        sale = x.sale,
                        description = x.description,
                        image = _imageService.GetImageUrl( x.image),
                        category = x.category.name,
                        categoryId = x.categoryId,

                    }).ToListAsync();
                else
                {
                    var hasID = await _databaseContext.CategoriesType
                   .Include(x => x.category).ThenInclude(y => y.products).FirstOrDefaultAsync(p => p.id == typeId);
                    //
                    if (hasID == null) return Ok(new Response() { statusCode = 404, message = "not found data", data = null }); 
                    //
                    products = hasID.category
                   .SelectMany(p => p.products)
                   .Select(x => new ProductDTO
                   {
                       id = x.id,
                       name = x.name,
                       price = x.price,
                       sale = x.sale,
                       description = x.description,
                       image = _imageService.GetImageUrl(x.image),
                       category = x.category.name,
                       categoryId = x.categoryId,

                   }).ToList();
                }
                //var products =  await _databaseContext.CategoriesType.SelectMany(c => c.category).SelectMany(p => p.products).ToListAsync();
               
                //var hasCategory = await _databaseContext.CategoriesType.Include(x => x.category).Where(p => p.category)
                //if (hasID == null) return Ok(new Response() { statusCode = 404, message = "not found data", data = null });               
                
                //fail
                //if (products.Count == 0) return Ok(new Response() { statusCode = 404, message = "not found data", data = null });
                if (categoryID != null) products = products.Where(p => p.categoryId == categoryID).ToList();
              
                
                switch (sorttype)
                {
                    case SortType.Default:
                        break;
                    case SortType.Ascending:
                        products = products.OrderBy(p => p.price).ToList();
                        break;
                    case SortType.Decreasing:
                        products = products.OrderByDescending(p => p.price).ToList();
                        break;
                    case SortType.Promotion:
                        products = products.Where(p => p.sale > 0).ToList();
                        break;
                    default:
                        break;
                }
                var descriptionAttribute = sorttype.GetType().GetMember(sorttype.ToString())[0]
                .GetCustomAttributes(typeof(DescriptionAttribute), inherit: false)[0]
                as DescriptionAttribute;
                //pagination
                var product = list_pagination(products, currentPage, pageSize);
                //set
                response.statusCode = 200;
                response.message = "success";
                response.data = new {descriptionAttribute.Description,product };
            }
            catch
            {
                return BadRequest(new Response { statusCode = 400, message = "fail", data = null });
            }

            return Ok(response);
        }
        [NonAction]
        public object list_pagination(IEnumerable<ProductDTO>? products, int currentPage, int pageSize)
        {
            //IEnumerable<ProductDTO>
            int total_item = products.Count();
            int skip = (currentPage - 1) * pageSize;
            int pageRanges = total_item % pageSize > 0
                ? (total_item / pageSize) + 1 : total_item / pageSize;

            products = products.Skip(skip).Take(pageSize).ToList();

            return new { products, pagination = new { total_item, currentPage, pageRanges } };
        }
        // GET: api/CategoryTypes/5

    }
}
