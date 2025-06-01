using System;
namespace QuizApp.Models.ViewModels
{
	public class QuizViewModel
	{
        public int QuizId { get; set; }
        public string Title { get; set; }
        public List<QuestionViewModel> Questions { get; set; }


        public int DurationInMinutes { get; set; }


    }
}

