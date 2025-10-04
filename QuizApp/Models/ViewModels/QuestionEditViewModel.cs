using System;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.ViewModels
{
	public class QuestionEditViewModel
	{
        public int Id { get; set; }

        [Required(ErrorMessage = "Sual mətni daxil edilməlidir")]
        public string Text { get; set; }

        public int CatalogId { get; set; }

        public string? ImageUrl { get; set; }

        public IFormFile? ImageFile { get; set; }

        public bool DeleteImage { get; set; }

        public List<AnswerEditViewModel> Answers { get; set; } = new List<AnswerEditViewModel>();


        [Required]
        public QuestionDifficulty Difficulty { get; set; }
    }
}

