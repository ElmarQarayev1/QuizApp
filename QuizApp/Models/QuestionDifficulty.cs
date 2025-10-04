using System;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public enum QuestionDifficulty
    {
        [Display(Name = "Asan")]
        Easy = 1,

        [Display(Name = "Orta")]
        Medium = 2,

        [Display(Name = "Çətin")]
        Hard = 3
    }
}

