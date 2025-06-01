using System;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Models;
using QuizApp.Models.ViewModels;

namespace QuizApp.Controllers
{
    [Authorize]
    public class QuizController:Controller
	{
        private readonly AppDbContext _context;

        public QuizController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Edit(int id)
        {
            var quiz = _context.Quizzes.FirstOrDefault(q => q.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            var model = new QuizCreateViewModel
            {
                Title = quiz.Title,
                DurationInMinutes=quiz.DurationInMinutes
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(int id, QuizCreateViewModel model)
        {
            

            var quiz = _context.Quizzes.FirstOrDefault(q => q.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

            quiz.Title = model.Title;
            quiz.DurationInMinutes = model.DurationInMinutes;
            _context.Quizzes.Update(quiz);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var quiz = _context.Quizzes
                .Include(q => q.QuizQuestions)
                .ThenInclude(qq => qq.Question)
                .ThenInclude(q => q.Answers)
                .FirstOrDefault(q => q.Id == id);

            if (quiz == null)
            {
                return NotFound();
            }

            var questions = quiz.QuizQuestions.Select(qq => qq.Question).ToList();

            foreach (var question in questions)
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
                _context.Questions.Remove(question);
            }

            _context.QuizQuestions.RemoveRange(quiz.QuizQuestions);
            _context.Quizzes.Remove(quiz);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult Index()
        {
            var quizzes = _context.Quizzes.ToList();
            return View(quizzes);
        }

        public IActionResult CreateQuiz()
        {
            var model = new QuizCreateViewModel
            {
                ExistingCatalogs = _context.Catalogs
                    .Select(q => new SelectListItem
                    {
                        Value = q.Id.ToString(),
                        Text = q.Title
                    }).ToList()
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult CreateQuiz(QuizCreateViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title) || model.DurationInMinutes == null || model.SelectedQuestionIds == null || !model.SelectedQuestionIds.Any())
            {
                model.ExistingCatalogs = _context.Catalogs
                    .Select(q => new SelectListItem
                    {
                        Value = q.Id.ToString(),
                        Text = q.Title
                    }).ToList();

                ModelState.AddModelError("", "Zəhmət olmasa bütün sahələri doldurun və ən azı bir sual seçin.");
                return View(model);
            }

            var quiz = new Quiz
            {
                Title = model.Title,
                CreatedAt = DateTime.Now,
                DurationInMinutes = model.DurationInMinutes,
                QuizQuestions = model.SelectedQuestionIds.Select(qid => new QuizQuestion
                {
                    QuestionId = qid
                }).ToList()
            };

            _context.Quizzes.Add(quiz);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> GetQuestionsByCatalogId(int catalogId)
        {
            var questions = await _context.Questions
                                          .Where(q => q.CatalogId == catalogId)
                                          .ToListAsync();

            return Json(questions);
        }


        public IActionResult TakeQuiz(int id)
        {
            var quizData = _context.Quizzes
                .Include(q => q.QuizQuestions)
                    .ThenInclude(qq => qq.Question)
                        .ThenInclude(qn => qn.Answers)
                .FirstOrDefault(q => q.Id == id);

            if (quizData == null)
                return NotFound();

          
            var distinctQuestions = quizData.QuizQuestions
                .GroupBy(qq => qq.Question.Id)
                .Select(g => g.First())
                .OrderBy(x => Guid.NewGuid())
                .ToList();

            var quizViewModel = new QuizViewModel
            {
                QuizId = quizData.Id,
                Title = quizData.Title,
                DurationInMinutes = quizData.DurationInMinutes,
                Questions = distinctQuestions.Select(qq => new QuestionViewModel
                {
                    QuestionId = qq.Question.Id,
                    Text = qq.Question.Text,
                    Image = qq.Question.ImageUrl,
                    Answers = qq.Question.Answers
                        .Select(a => new AnswerViewModel
                        {
                            AnswerId = a.Id,
                            Text = a.Text
                        }).ToList()
                }).ToList()
            };

            return View(quizViewModel);
        }


        public IActionResult CalculateResult(QuizSubmissionViewModel model)
        {
            if (model.Answers == null || model.Answers.Count == 0)
            {
                ModelState.AddModelError("", "Herhangi bir cevap alınamadı!");
                return RedirectToAction("Index");
            }

            int correctAnswers = 0;
            var wrongQuestions = new List<QuestionResultViewModel>();
            var correctQuestions = new List<QuestionResultViewModel>();  

            foreach (var answer in model.Answers)
            {
                var question = _context.Questions.Include(q => q.Answers).FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null) continue;

                var correct = question.Answers.FirstOrDefault(a => a.IsCorrect);
                var selected = question.Answers.FirstOrDefault(a => a.Id == answer.SelectedAnswerId);

                bool isCorrect = correct != null && selected != null && correct.Id == selected.Id;

                if (isCorrect)
                {
                    correctAnswers++;
                    correctQuestions.Add(new QuestionResultViewModel
                    {
                        QuestionText = question.Text,
                        Answers = question.Answers.Select(a => a.Text).ToList(),
                        SelectedAnswer = selected?.Text ?? "Seçilməyib",
                        CorrectAnswer = correct?.Text ?? "Tapılmadı"
                    });
                }
                else
                {
                    wrongQuestions.Add(new QuestionResultViewModel
                    {
                        QuestionText = question.Text,
                        Answers = question.Answers.Select(a => a.Text).ToList(),
                        SelectedAnswer = selected?.Text ?? "Seçilməyib",
                        CorrectAnswer = correct?.Text ?? "Tapılmadı"
                    });
                }
            }

            int totalQuestions = model.Answers.Count;
            int wrongAnswers = totalQuestions - correctAnswers;

            var result = new QuizResultViewModel
            {
                QuizId = model.QuizId,
                CorrectAnswers = correctAnswers,
                WrongAnswers = wrongAnswers,
                TotalQuestions = totalQuestions,
                UserName = User.Identity.Name,
                WrongQuestions = wrongQuestions,
                CorrectQuestions = correctQuestions  
            };

            TempData["WrongQuestions"] = JsonSerializer.Serialize(result.WrongQuestions);
            TempData["CorrectQuestions"] = JsonSerializer.Serialize(result.CorrectQuestions);  
            return View("Result", result);
        }


        public IActionResult ReviewWrongQuestions()
        {
            var wrongQuestionsData = TempData["WrongQuestions"] as string;
            var correctQuestionsData = TempData["CorrectQuestions"] as string;

            var wrongQuestions = !string.IsNullOrEmpty(wrongQuestionsData)
                ? JsonSerializer.Deserialize<List<QuestionResultViewModel>>(wrongQuestionsData)
                : new List<QuestionResultViewModel>();

            var correctQuestions = !string.IsNullOrEmpty(correctQuestionsData)
                ? JsonSerializer.Deserialize<List<QuestionResultViewModel>>(correctQuestionsData)
                : new List<QuestionResultViewModel>();

            var model = new QuizReviewViewModel
            {
                WrongQuestions = wrongQuestions,
                CorrectQuestions = correctQuestions
            };

            return View(model);
        }

        public IActionResult CreateQuestions()
        {
            ViewBag.Catalogs = _context.Catalogs.ToList();
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult CreateQuestions(List<QuestionCreateViewModel> models)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Catalogs = _context.Catalogs.ToList();
                return View(models);
            }

            foreach (var model in models)
            {
                var catalog = _context.Catalogs.FirstOrDefault(c => c.Id == model.CatalogId);

                if (catalog == null)
                {
                    ModelState.AddModelError("", "Kataloq tapılmadı.");
                    ViewBag.Catalogs = _context.Catalogs.ToList();
                    return View(models);
                }

                string? imageUrl = null;
                if (model.ImageFile != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/questions");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ImageFile.CopyTo(fileStream);
                    }

                    imageUrl = "/uploads/questions/" + uniqueFileName;
                }

                var question = new Question
                {
                    Text = model.Text,
                    CatalogId = catalog.Id,
                    ImageUrl = imageUrl,
                    Answers = model.Answers.Select(a => new Answer
                    {
                        Text = a.Text,
                        IsCorrect = a.IsCorrect
                    }).ToList()
                };

                _context.Questions.Add(question);
            }

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Xəta baş verdi: " + ex.Message);
                return View(models);
            }

            return RedirectToAction("Index");
        }


    }
}

