using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace QuizApp.Models
{
	public class Question
	{
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Text { get; set; }

        public int? QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }

        public int? CatalogId { get; set; }
        public virtual Catalog Catalog { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public QuestionDifficulty Difficulty { get; set; } = QuestionDifficulty.Medium;

        public virtual ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();

        public ICollection<QuizQuestion> QuizQuestions { get; set; }



    }
}

