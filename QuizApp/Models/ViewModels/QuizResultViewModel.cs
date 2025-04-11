using System;
namespace QuizApp.Models.ViewModels
{

    public class QuizResultViewModel
    {
        public int QuizId { get; set; }
        public int CorrectAnswers { get; set; }
        public int WrongAnswers { get; set; }
        public int TotalQuestions { get; set; }
    }

}

