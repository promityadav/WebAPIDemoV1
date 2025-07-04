﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPIDemo.Data;
using WebAPIDemo.Models.Repositories;

namespace WebAPIDemo.Filters.ActionFilters
{
    public class Shirt_ValidateShirtIdFilterAttribute: ActionFilterAttribute
    {
        private ApplicationDbContext db;
        public Shirt_ValidateShirtIdFilterAttribute(ApplicationDbContext db)
        {
            this.db = db;   
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            var shirtId = context.ActionArguments["id"] as int?;
            if (shirtId.HasValue)
            { 
                if (shirtId.Value <= 0)
                {
                    context.ModelState.AddModelError("shirtId", "Shirt ID is invalid");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status400BadRequest
                       
                    };
                    context.Result=new BadRequestObjectResult(problemDetails);
                }
                else 
                {
                    var shirt = db.Shirt.Find(shirtId.Value);
                    if (shirt == null)
                    {
                        context.ModelState.AddModelError("shirtId", "Shirt ID doesn't exist.");
                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Status = StatusCodes.Status404NotFound

                        };
                        context.Result = new NotFoundObjectResult(problemDetails);
                    }
                    else
                    {
                        context.HttpContext.Items["shirt"] = shirt;
                    }
                    
                }
            }
        }
    }
}
