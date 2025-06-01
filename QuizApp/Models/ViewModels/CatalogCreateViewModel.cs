using System;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.ViewModels
{
	public class CatalogCreateViewModel
	{
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
    }
}

