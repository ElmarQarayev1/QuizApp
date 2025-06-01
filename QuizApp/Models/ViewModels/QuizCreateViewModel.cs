using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuizApp.Models.ViewModels
{
	public class QuizCreateViewModel
	{
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Range(1, 180)]
        public int DurationInMinutes { get; set; }

        public int? SelectedQuizId { get; set; } 

        public List<int> SelectedQuestionIds { get; set; } = new(); 

        public List<SelectListItem> ExistingCatalogs { get; set; }

        public int? SelectedCatalogId { get; set; }

        public IFormFile? Image { get; set; }


    }


}





