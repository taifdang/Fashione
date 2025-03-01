using eCommerce_API.DTO;
using eCommerce_API.Models;
using eCommerce_API.Service;
using eCommerce_API.Translate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace eCommerce_API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly ICategoryService _categoryService;
        public CategoriesController(DatabaseContext databaseContext, ICategoryService categoryService)
        {
            _databaseContext = databaseContext;
            _categoryService = categoryService;
        }

        #region DTO
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> getAllDTO()
        {
            var response = new Response();
            var data = await _categoryService.GetAll();
            if (data == null) return Ok(new Response() { statusCode = 404, message = "not found data", data = null});
            //custom list data
            //var _getData = data.Select(c => new {c.id,c.name}).ToList();
            response.statusCode = 200;
            response.message = "success";
            response.data = data;
            return Ok(response);         
        }
        
        [HttpGet("get-product-by-id")]
        public async Task<ActionResult<Category>> getSingleDTO(int id, int currentPage, int pageSize)
        {
            var response = new Response();
            try
            {
                var data = await _categoryService.GetId(id, currentPage, pageSize);
                if (data == null) return Ok(new Response() { statusCode = 404, message = "not found data", data = null });
                //set infomation page
                var totalItem = _databaseContext.Products.Where(p => p.categoryId == id).Count();
                var pageRange = _categoryService.GetPageRanges(pageSize, totalItem);

                response.statusCode = 200;
                response.message = "success";
                response.data = new { data, pagination = new { totalItem, currentPage, pageRange } };
            }
            catch
            {
                response.statusCode = 500;
                response.message = "fail";
                response.data = null;
            }
           
            
           return Ok(response);
            
        }      
        #endregion
    }
}
