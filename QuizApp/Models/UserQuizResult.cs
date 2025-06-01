using System;
namespace QuizApp.Models
{
	public class UserQuizResult
	{
        public int Id { get; set; } 

        public string UserId { get; set; } 
        public int QuizId { get; set; } 
        public int Score { get; set; } 

        public DateTime CompletedAt { get; set; } = DateTime.Now;

        public virtual Quiz Quiz { get; set; }
    }
}

