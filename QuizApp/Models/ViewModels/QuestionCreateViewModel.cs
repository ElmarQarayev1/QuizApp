using System;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.ViewModels
{
	public class QuestionCreateViewModel
	{
        [Required]
        public string Text { get; set; }

        [Required]
        public int CatalogId { get; set; }

        public IFormFile? ImageFile { get; set; }


        public List<AnswerCreateViewModel> Answers { get; set; } = new List<AnswerCreateViewModel>();

        [Required]
        public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Medium;


    }
}


