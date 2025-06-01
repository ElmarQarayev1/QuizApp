using System;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
	public class Quiz
	{
        public int Id { get; set; } 

        [Required]
        [StringLength(200)]
        public string Title { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.Now; 

        public virtual ICollection<Question> Questions { get; set; }

        public int DurationInMinutes { get; set; }

        public ICollection<QuizQuestion> QuizQuestions { get; set; }
    }
}


