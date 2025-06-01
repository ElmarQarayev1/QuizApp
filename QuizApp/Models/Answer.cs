using System;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
	public class Answer
	{
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Text { get; set; }

        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }

        public bool IsCorrect { get; set; }
    }
}

