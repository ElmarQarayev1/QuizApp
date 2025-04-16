using System;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]
        public IActionResult Edit(int id, QuizCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

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
       
        public IActionResult Delete(int id)
        {
            var quiz = _context.Quizzes.FirstOrDefault(q => q.Id == id);
            if (quiz == null)
            {
                return NotFound();
            }

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
                CreatedAt = DateTime.Now,
                DurationInMinutes = model.DurationInMinutes
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
                    DurationInMinutes = q.DurationInMinutes,
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


    }
}

