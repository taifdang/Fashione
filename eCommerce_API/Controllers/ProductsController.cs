using AutoMapper;
using eCommerce_API.DTO;
using eCommerce_API.Models;
using eCommerce_API.Service;
using eCommerce_API.Translate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Xml.Linq;


namespace eCommerce_API.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;   
        private readonly IProductService _productService;
        private readonly ImageService _imageService;
        public ProductsController(DatabaseContext databaseContext,IMapper mapper,IProductService productService,ImageService imageService)
        {
            this._databaseContext = databaseContext;
            this._mapper = mapper;            
            this._productService = productService;
            this._imageService = imageService;
        }  
        #region PRODUCT_DT0     
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> getAllDTO()
        {
            var response = new Response();
            try
            {
                var data = await _productService.GetAll();
                if (data == null) return Ok(new Response() { statusCode = 404, message = "not found data", data = null });
                //success                          
                response.statusCode = 200;
                response.message = "success";
                response.data = data;
            }
            catch
            {
                response.statusCode = 500;
                response.message = "fail";
                response.data = null;
            }                             
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> getSingleDTO(int id)
        {
            var response = new Response();
            try
            {
                var data = await _productService.GetId(id);
                //fail
                if (data == null) return Ok(new Response() { statusCode = 404, message = "not found data", data = null });
                //sucess
                response.statusCode = 200;
                response.message = "success";
                response.data = data;   
            }
            catch
            {
                response.statusCode = 500;
                response.message = "fail";
                response.data = null;
            }           
            return Ok(response);
        }
        [HttpGet("get-alls")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> getAllPagination(int currentPage,int pageSize)
        {
            var response = new Response();
            try
            {
                var data = await _productService.GetAllPagination(currentPage, pageSize);
                //fail
                if(data == null) return Ok(new Response() { statusCode = 404, message = "not found data", data = null });
                //success
                //set infomation page
                int totalItem = _productService.GetTotalProduct("");
                int pageRanges = _productService.GetPageRanges(pageSize, totalItem);
                //response
                response.statusCode = 200;
                response.message = "success";   
                response.data = new {data,pagination= new {totalItem,currentPage,pageRanges}};
            }
            catch
            {
                response.statusCode = 500;
                response.message = "fail";
                response.data = null;
            }
           
            return Ok(response);          
        
        }      
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> searchDTO(string? keyword,int currentPage,int pageSize)
        {
            var response = new Response();

            try
            {
                if (string.IsNullOrEmpty(keyword))
                {
                    response.data = await _productService.GetAllPagination(currentPage, pageSize);
                }
                else
                {
                    response.data = await _productService.Search(keyword, currentPage, pageSize);
                    if (response.data == null) return Ok(new Response() { statusCode=404,message= "not found data",data= null });
                    
                }
                //set infomation page
                var totalItem = _productService.GetTotalProduct(keyword);
                var pageRanges = _productService.GetPageRanges(pageSize, totalItem);
                //response
                response.statusCode = 200;
                response.message = "success";
                response.data = new { response.data, pagination = new { totalItem, currentPage, pageRanges } };

            }
            catch
            {
                response.statusCode = 500;
                response.message = "fail";
                response.data = null;
            }
           
            return Ok(response);
        }
        [HttpPost("add")]
        public async Task<ActionResult> addDTO([FromForm]ProductImageDTO IProductDTO)
        {
            var response = new Response();
            try
            {
                var data = await _productService.Add(IProductDTO);
                if (data == null) return Ok(new Response() { statusCode = 400, message = "bad request", data = null });
                //success
                response.data = data;
                response.statusCode = 201;
                response.message = "Success";               
            }
            catch
            {
                response.statusCode = 500;
                response.message = "fail";
                response.data = null;
            }           
            return Ok(response);
           
        }
        [HttpPut("update")]
        public async Task<ActionResult> updateDTO([FromForm] ProductImageDTO IProductDTO, int id)
        {
            var response = new Response();
            try
            {                               
                if(id != IProductDTO.id) return Ok(new Response() { statusCode = 400, message = "not match id", data = null });
                //
                var data = await _productService.GetId(id);
                if (data == null) return Ok(new Response() { statusCode = 400, message = "not match id", data = null });
                //
                var new_data = await _productService.Update(IProductDTO);
                if (new_data == null) return Ok(new Response() { statusCode = 404, message = "not found data", data = null });
                //
                //success              
                response.statusCode = 201;
                response.message = "Success";
                response.data = data;
            }
            catch
            {
                response.statusCode = 500;
                response.message = "fail";
                response.data = null;
            }                    
            return Ok(response);
        }
        [HttpDelete("delete")]
        public async Task<ActionResult> deleteDTO(int id)
        {
            var response = new Response();
            try
            {
                var result = await _productService.GetId(id);
                if (result == null) return Ok(new Response() { statusCode = 404, message = "not found data", data = null });
                //
                await _productService.Delete(id);

                response.statusCode = 200;
                response.message = "Success";
                response.data = null;
            }
            catch
            {
                response.statusCode = 500;
                response.message = "fail";
                response.data = null;
            }
            
            return Ok(response);
        }      
        [HttpGet("sort-product")]
        public async Task<ActionResult> sort2DTO(int? categoryID,SortType sorttype,int currentPage, int pageSize)
        {
            var response = new Response(); 
            try
            {
                var products = await _databaseContext.CategoriesType
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
                //
                //var products = await _productService.GetAll();
                
                //fail
                if (products == null) return Ok(new Response() { statusCode = 404, message = "not found data", data = null });
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
                response.data = new { descriptionAttribute.Description, product };
            }
            catch
            {
                return BadRequest(new Response{ statusCode = 400, message = "fail", data = null });
            }
           
            return Ok(response);
        }
        [NonAction]
        public object list_pagination(IEnumerable<ProductDTO>? products,int currentPage, int pageSize)
        {
            //IEnumerable<ProductDTO>
            int total_item = products.Count();
            int skip = (currentPage - 1)*pageSize;           
            int pageRanges = total_item % pageSize > 0 
                ? (total_item / pageSize) + 1: total_item / pageSize;

            products =  products.Skip(skip).Take(pageSize).ToList();

            return new { products, pagination = new { total_item, currentPage, pageRanges } }; 
        }
        [HttpGet("download-file")]
        public async Task<ActionResult> downloadFile(string filename)
        {          
            var filePath = Path.Combine(Directory.GetCurrentDirectory(),"Uploads" ,filename);
            if (System.IO.File.Exists(filePath))
            {
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(filePath, out var contentType))
                {
                    contentType = "application/octet-streeam";
                }
                var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return  File(bytes, contentType, filename);
            }        
            return BadRequest();          
        }

        #endregion ___________________________________________________________________________
    }
}
