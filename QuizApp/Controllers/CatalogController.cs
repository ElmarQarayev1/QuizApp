using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Models;
using QuizApp.Models.ViewModels;

namespace QuizApp.Controllers
{
    public class CatalogController : Controller
    {
        private readonly AppDbContext _context;

        public CatalogController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var catalogs = _context.Catalogs.ToList();
            return View(catalogs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(CatalogCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var catalog = new Catalog
            {
                Title = model.Title
            };

            _context.Catalogs.Add(catalog);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            var catalog = _context.Catalogs.FirstOrDefault(c => c.Id == id);
            if (catalog == null) return NotFound();

            var model = new CatalogEditViewModel
            {
                Id = catalog.Id,
                Title = catalog.Title
            };

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var catalog = _context.Catalogs
                                  .FirstOrDefault(c => c.Id == id);

            if (catalog == null)
                return NotFound();


            var questions = _context.Questions
                                    .Where(q => q.CatalogId == id)
                                    .ToList();

            var model = new CatalogDetailsViewModel
            {
                Catalog = catalog,
                Questions = questions
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(CatalogEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var catalog = _context.Catalogs.FirstOrDefault(c => c.Id == model.Id);
            if (catalog == null) return NotFound();

            catalog.Title = model.Title;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var catalog = await _context.Catalogs
                                        .Include(c => c.Questions)
                                        .ThenInclude(q => q.Answers)
                                        .FirstOrDefaultAsync(c => c.Id == id);

            if (catalog == null) return NotFound();

            foreach (var question in catalog.Questions)
            {
                if (!string.IsNullOrEmpty(question.ImageUrl))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", question.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Answers.RemoveRange(question.Answers);
            }

            _context.Questions.RemoveRange(catalog.Questions);
            _context.Catalogs.Remove(catalog);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


    }
}


