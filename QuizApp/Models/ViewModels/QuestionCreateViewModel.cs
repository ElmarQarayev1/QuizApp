using System;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.ViewModels
{
	public class QuestionCreateViewModel
	{
        [Required]
        public string Text { get; set; }

        [Required]
        public int QuizId { get; set; }

        public List<AnswerCreateViewModel> Answers { get; set; } = new List<AnswerCreateViewModel>();


    }
}


