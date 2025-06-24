using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using WebAPIDemo.Data;
using WebAPIDemo.Models;
using WebAPIDemo.Models.Repositories;

namespace WebAPIDemo.Filters.ActionFilters
{
    public class Shirt_ValidateCreateShirtFilterAttribute: ActionFilterAttribute    
    {
        private ApplicationDbContext db;

        public Shirt_ValidateCreateShirtFilterAttribute(ApplicationDbContext db)
        {
            this.db = db;  
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var shirt = context.ActionArguments["shirt"] as Shirt;
            if (shirt == null) {
                context.ModelState.AddModelError("Shirt", "Shirt object is null");
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest
                };
                context.Result=new BadRequestObjectResult(problemDetails);
            }
            else
            {
                var existingShirt = db.Shirt.FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(shirt.Brand) &&
                    !string.IsNullOrWhiteSpace(x.Brand) &&
                    x.Brand.ToLower() == shirt.Brand.ToLower() &&
                    !string.IsNullOrWhiteSpace(shirt.Gender) &&
                    !string.IsNullOrWhiteSpace(x.Gender) &&
                    x.Gender.ToLower() == shirt.Gender.ToLower() &&
                    !string.IsNullOrWhiteSpace(shirt.color) &&
                    !string.IsNullOrWhiteSpace(x.color) &&
                    x.color.ToLower() == shirt.color.ToLower() &&
                    shirt.Size.HasValue &&
                    x.Size.HasValue &&
                    shirt.Size.Value == x.Size.Value);
                if (existingShirt != null) {
                    context.ModelState.AddModelError("Shirt", "Shirt already exists.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                }
            }
        }
    }
}
