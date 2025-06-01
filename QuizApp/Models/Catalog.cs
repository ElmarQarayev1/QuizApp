using System;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
	public class Catalog
	{
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public List<Question> Questions { get; set; } = new List<Question>();


    }
}

