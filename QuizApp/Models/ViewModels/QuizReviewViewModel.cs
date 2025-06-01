using System;
namespace QuizApp.Models.ViewModels
{
	public class QuizReviewViewModel
	{
        public List<QuestionResultViewModel> CorrectQuestions { get; set; }
        public List<QuestionResultViewModel> WrongQuestions { get; set; }
    }
}

