﻿using Microsoft.AspNetCore.Mvc;

namespace E_BookingFutsal.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}