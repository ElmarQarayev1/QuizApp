using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizApp.Models;

namespace QuizApp.Data
{
	public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<UserQuizResult> UserQuizResults { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<QuizQuestion> QuizQuestions { get; set; }


        public DbSet<Catalog> Catalogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<QuizQuestion>()
                .HasKey(qq => new { qq.QuizId, qq.QuestionId });

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(qq => qq.Quiz)
                .WithMany(q => q.QuizQuestions)
                .HasForeignKey(qq => qq.QuizId);

            modelBuilder.Entity<QuizQuestion>()
                .HasOne(qq => qq.Question)
                .WithMany(q => q.QuizQuestions)
                .HasForeignKey(qq => qq.QuestionId);
        }

    }
}

