using Microsoft.AspNetCore.Mvc;
using ShopTARge24.Core.Domain;
using ShopTARge24.Core.Dto;
using ShopTARge24.Core.ServiceInterface;
using ShopTARge24.Data;
using ShopTARge24.Models.Kindergarten;


namespace ShopTARge24.Controllers
{
    public class KindergartenController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}