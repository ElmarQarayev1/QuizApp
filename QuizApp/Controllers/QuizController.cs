using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            var quizzes = _context.Quizzes.ToList();
            return View(quizzes);
        }

        public IActionResult CreateQuiz()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateQuiz(QuizCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var quiz = new Quiz
            {
                Title = model.Title,
                CreatedAt = DateTime.Now
            };

            _context.Quizzes.Add(quiz);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult TakeQuiz(int id)
        {
            var quiz = _context.Quizzes
                .Where(q => q.Id == id)
                .Select(q => new QuizViewModel
                {
                    QuizId = q.Id,
                    Title = q.Title,
                    Questions = q.Questions.Select(qn => new QuestionViewModel
                    {
                        QuestionId = qn.Id,
                        Text = qn.Text,
                        Answers = qn.Answers.Select(a => new AnswerViewModel
                        {
                            AnswerId = a.Id,
                            Text = a.Text
                        }).ToList()
                    }).ToList()
                }).FirstOrDefault();

            if (quiz == null)
                return NotFound();

            return View(quiz);
        }

      
        public IActionResult CreateQuestion()
        {
            ViewBag.Quizzes = _context.Quizzes.ToList(); 
            return View();
        }

        [HttpPost]
        public IActionResult CreateQuestions(List<QuestionCreateViewModel> models)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Quizzes = _context.Quizzes.ToList();
                return View(models);
            }

            foreach (var model in models)
            {
                var question = new Question
                {
                    Text = model.Text,
                    QuizId = model.QuizId,
                    Answers = model.Answers.Select(a => new Answer
                    {
                        Text = a.Text,
                        IsCorrect = a.IsCorrect
                    }).ToList()
                };

                _context.Questions.Add(question);
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult CalculateResult(QuizSubmissionViewModel model)
        {
            if (model.Answers == null || model.Answers.Count == 0)
            {
                ModelState.AddModelError("", "Herhangi bir cevap alınamadı!");
                return RedirectToAction("Index");
            }

            int correctAnswers = 0;

            foreach (var answer in model.Answers)
            {
                bool isCorrect = _context.Answers
                    .Where(a => a.QuestionId == answer.QuestionId && a.IsCorrect)
                    .Any(a => a.Id == answer.SelectedAnswerId);

                if (isCorrect)
                {
                    correctAnswers++;
                }
            }

            int totalQuestions = model.Answers.Count;
            int wrongAnswers = totalQuestions - correctAnswers;

            var result = new QuizResultViewModel
            {
                QuizId = model.QuizId,
                CorrectAnswers = correctAnswers,
                WrongAnswers = wrongAnswers,
                TotalQuestions = totalQuestions
            };

            return View("Result", result);
        }
    }
}

