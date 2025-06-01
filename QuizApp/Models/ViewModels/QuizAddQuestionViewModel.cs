using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuizApp.Models.ViewModels
{
    public class QuizAddQuestionsViewModel
    {
        public int QuizId { get; set; }
        public List<SelectListItem> AvailableQuestions { get; set; }
        public List<int> SelectedQuestionIds { get; set; } = new List<int>();
    }

}

