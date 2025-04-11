using System;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
	public class Question
	{
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Text { get; set; }

        public int QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }

        public virtual ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();
    }
}

