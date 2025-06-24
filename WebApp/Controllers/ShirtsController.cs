using Microsoft.AspNetCore.Mvc;
using WebApp.Data;
using WebApp.Models;
using WebApp.Models.Repositories;

namespace WebApp.Controllers
{
    public class ShirtsController : Controller
    {
        private readonly IWebApiExecuter WebApiExecuter;

        public ShirtsController(IWebApiExecuter webApiExecuter)
        {
            WebApiExecuter = webApiExecuter;
        }


        public async Task<IActionResult> Index()
        {
            return View(await WebApiExecuter.InvokeGet<List<Shirt>>("shirts"));
        }
        public IActionResult CreateShirt()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateShirt(Shirt shirt)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await WebApiExecuter.InvokePost("shirts", shirt);
                    if (response != null)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (WebApiException ex)
                {
                    HandelWebApiException(ex);


                }
                
            }
            return View(shirt);
        }
        public async Task<IActionResult> UpdateShirt(int ShirtId)
        {
            try
            {
                var shirt = await WebApiExecuter.InvokeGet<Shirt>($"shirts/{ShirtId}");
                if (shirt != null)
                {
                    return View(shirt);
                }
            }
            catch (WebApiException ex)
            {
                HandelWebApiException(ex);
            }
            
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateShirt(Shirt shirt)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await WebApiExecuter.InvokePut($"shirts/{shirt.shirtId}", shirt);

                    return RedirectToAction(nameof(Index));

                }
            }
            catch (WebApiException ex)
            {

                HandelWebApiException(ex);
                return View();
            }

            
            return View(shirt);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteShirt([FromForm]int ShirtId)
        {
            try
            {
                await WebApiExecuter.InvokeDelete($"shirts/{ShirtId}");
                return RedirectToAction(nameof(Index));
            }
            catch (WebApiException ex)
            {

                HandelWebApiException(ex);
                return View(nameof(Index),
                    await WebApiExecuter.InvokeGet<List<Shirt>>("Shirts"));
            }
           
        }
        private void HandelWebApiException(WebApiException ex)
        {
            if (ex.errorResponse != null && ex.errorResponse.Errors != null && ex.errorResponse.Errors.Count() > 0)
            {
                foreach (var error in ex.errorResponse.Errors)
                {
                    ModelState.AddModelError(error.Key, string.Join("; ", error.Value));
                }
            }
        }
    }
}
