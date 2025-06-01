using System;
namespace QuizApp.Models.ViewModels
{
    public class QuestionResultViewModel
    {
        public string QuestionText { get; set; }
        public List<string> Answers { get; set; }
        public string SelectedAnswer { get; set; }
        public string CorrectAnswer { get; set; }
    }

}

