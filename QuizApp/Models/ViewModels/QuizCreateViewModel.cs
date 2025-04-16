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

        [Display(Name = "Vaxt (dəqiqə ilə)")]
        [Range(1, 180, ErrorMessage = "Vaxt 1 ilə 180 dəqiqə arasında olmalıdır")]
        public int DurationInMinutes { get; set; }


      
    }


}


