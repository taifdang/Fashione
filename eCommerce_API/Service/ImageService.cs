namespace eCommerce_API.Service
{
    public class ImageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ImageService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetImageUrl(string image)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/upload/{image}";
        }
    }
}
