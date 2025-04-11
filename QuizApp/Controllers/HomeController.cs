using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Models;

namespace QuizApp.Controllers;

[Authorize]
public class HomeController : Controller
{
      

    public IActionResult Index()
    {
        return View();
    }

 
}

