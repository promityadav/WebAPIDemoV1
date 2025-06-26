using Microsoft.EntityFrameworkCore;
using WebAPIDemo.Authority;
using WebAPIDemo.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ShirtStoreManagement"));
}
);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();      // required for Swagger
builder.Services.AddSwaggerGen();
var app = builder.Build();
//app.UseRouting();
// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();        // exposes /swagger/v1/swagger.json
    app.UseSwaggerUI();      // exposes Swagger UI at /swagger
}
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
