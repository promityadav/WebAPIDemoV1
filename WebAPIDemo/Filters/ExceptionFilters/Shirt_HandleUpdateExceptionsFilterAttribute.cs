using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPIDemo.Data;
using WebAPIDemo.Models.Repositories;

namespace WebAPIDemo.Filters.ExceptionFilters
{
    public class Shirt_HandleUpdateExceptionsFilterAttribute: ExceptionFilterAttribute
    {
        private readonly ApplicationDbContext db;

        public Shirt_HandleUpdateExceptionsFilterAttribute(ApplicationDbContext db)
        {
            this.db = db;
        }
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            var StrshirtID = context.RouteData.Values["id"] as string;
            if (int.TryParse(StrshirtID, out int shirtId))
            {
                if (db.Shirt.FirstOrDefault(x => x.shirtId == shirtId) == null)
                {
                    context.ModelState.AddModelError("ShirtId", "Shirt does not exists anymore.");
                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Status = StatusCodes.Status404NotFound
                    };
                    context.Result=new NotFoundObjectResult(problemDetails);
                }
            }
        }
    }
}
