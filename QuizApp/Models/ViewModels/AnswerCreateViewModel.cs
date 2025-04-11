using System;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.ViewModels
{
	public class AnswerCreateViewModel
	{
        [Required]
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}

