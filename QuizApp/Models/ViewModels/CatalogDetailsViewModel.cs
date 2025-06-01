using System;
namespace QuizApp.Models.ViewModels
{
	public class CatalogDetailsViewModel
	{
        public Catalog Catalog { get; set; }
        public List<Question> Questions { get; set; }
    }
}

