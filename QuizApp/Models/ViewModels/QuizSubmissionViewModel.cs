using System;
namespace QuizApp.Models.ViewModels
{
	public class QuizSubmissionViewModel
	{
      
        public int QuizId { get; set; }
        public List<UserAnswerViewModel> Answers { get; set; } = new List<UserAnswerViewModel>();
        public string UserName { get; set; }
    }
}

