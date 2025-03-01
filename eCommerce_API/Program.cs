using AutoMapper;
using eCommerce_API.DTO;
using eCommerce_API;
using eCommerce_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using eCommerce_API.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//#6.add httpcontext
builder.Services.AddHttpContextAccessor();
//#7.auto mapper
builder.Services.AddAutoMapper(typeof(MapingProfile)); 
//#1.connect database
builder.Services.AddDbContext<DatabaseContext>(p=>p.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));
//#5.add scopes
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
//#2.Add cors
builder.Services.AddCors(p => p.AddPolicy("MyCors", build =>
{
    build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//#3.Cors
app.UseCors("MyCors");
//#4.StaticFiles
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),"Uploads")),RequestPath = "/upload"           
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
