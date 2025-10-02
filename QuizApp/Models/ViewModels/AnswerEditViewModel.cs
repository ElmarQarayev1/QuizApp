using System;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.ViewModels
{
	public class AnswerEditViewModel
	{
        public int Id { get; set; }

        [Required(ErrorMessage = "Cavab mətni daxil edilməlidir")]
        public string Text { get; set; }

        public bool IsCorrect { get; set; }
    }
}

