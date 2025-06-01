using System;
namespace QuizApp.Models.ViewModels
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public string Text { get; set; }
        public List<AnswerViewModel> Answers { get; set; }

        public string Image { get; set; }

    }

}

