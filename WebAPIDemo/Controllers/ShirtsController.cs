using Microsoft.AspNetCore.Mvc;
using WebAPIDemo.Attributes;
using WebAPIDemo.Data;
using WebAPIDemo.Filters.ActionFilters;
using WebAPIDemo.Filters.AuthFilters;
using WebAPIDemo.Filters.ExceptionFilters;
using WebAPIDemo.Models;
using WebAPIDemo.Models.Repositories;

namespace WebAPIDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [JwtTokenAuthFilter]
    public class ShirtsController : ControllerBase
    {
        private readonly ApplicationDbContext db;
        public ShirtsController(ApplicationDbContext db)
        {
            this.db= db;
        }

        [HttpGet]
        [RequiredClaim("read","true")]
        public IActionResult GetShirts()
        {
            return Ok(db.Shirt.ToList());
        }
        [HttpGet("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [RequiredClaim("read", "true")]
        public IActionResult GetShirtById(int id)
        {

            return Ok(HttpContext.Items["shirt"]);
        }
        [HttpPost]
        [TypeFilter(typeof(Shirt_ValidateCreateShirtFilterAttribute))]
        [RequiredClaim("write", "true")]
        public IActionResult CreateShirt([FromBody] Shirt shirt)
        {
            this.db.Add(shirt);
            this.db.SaveChanges();

            ShirtRepository.AddShirt(shirt);
            return CreatedAtAction(nameof(GetShirtById), new { id = shirt.shirtId }, shirt);
        }
        [HttpPut("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [ShirtValidateUpdateShirtFilter]
        [TypeFilter(typeof(Shirt_HandleUpdateExceptionsFilterAttribute))]
        [RequiredClaim("write", "true")]
        public IActionResult UpdateShirt(int id, Shirt shirt)
        {
            var shirtToUpdate = HttpContext.Items["shirt"] as Shirt;
            shirtToUpdate.Brand = shirt.Brand;
            shirtToUpdate.Price = shirt.Price;
            shirtToUpdate.Size = shirt.Size;
            shirtToUpdate.color = shirt.color;
            shirtToUpdate.Gender = shirt.Gender;

            db.SaveChanges();

            return NoContent();

        }
        [HttpDelete("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [RequiredClaim("delete", "true")]
        public IActionResult DeleteShirt(int id)
        {
            var shirtToDelete = HttpContext.Items["shirt"] as Shirt;

            db.Shirt.Remove(shirtToDelete);
            db.SaveChanges();
            return Ok(shirtToDelete);
        }
    }
}
