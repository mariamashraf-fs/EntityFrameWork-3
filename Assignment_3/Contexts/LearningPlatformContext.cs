using Assignment_3.Enums;
using Assignment_3.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment_3.Contexts
{
    public class LearningPlatformContext : DbContext
    {
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-15UIE3F\\SQLEXPRESS;Database=LearningPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearningPlatformContext).Assembly);


            // Seed Data
            modelBuilder.Entity<Instructor>().HasData(
                new Instructor { InstructorId = 1, FullName = "Ahmed Samir", Email = "ahmed.samir@example.com", Bio = "Senior .NET developer and trainer", JoinDate = new DateTime(2023, 1, 10) },
                new Instructor { InstructorId = 2, FullName = "Nour Khaled", Email = "nour.khaled@example.com", Bio = null, JoinDate = new DateTime(2023, 6, 5) },
                new Instructor { InstructorId = 3, FullName = "Mona Adel", Email = "mona.adel@example.com", Bio = "Front-end engineer", JoinDate = new DateTime(2024, 2, 20) }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Programming", Description = "All programming topics", ParentCategoryId = null },
                new Category { CategoryId = 2, Name = "Backend", Description = "Server-side development", ParentCategoryId = 1 },
                new Category { CategoryId = 3, Name = "ASP.NET", Description = null, ParentCategoryId = 2 },
                new Category { CategoryId = 4, Name = "Frontend", Description = "Client-side development", ParentCategoryId = 1 },
                new Category { CategoryId = 5, Name = "Design", Description = "UI and graphic design", ParentCategoryId = null },
                new Category { CategoryId = 6, Name = "Data Science", Description = null, ParentCategoryId = null }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 1, Title = "C# Fundamentals", Description = "Learn C# from scratch", Price = 49.99m, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2025, 1, 15), InstructorId = 1, CategoryId = 1 },
                new Course { CourseId = 2, Title = "ASP.NET Core Web API", Description = "Build REST APIs", Price = 99.99m, Level = CourseLevel.Intermediate, IsPublished = true, PublishedDate = new DateTime(2025, 3, 1), InstructorId = 1, CategoryId = 3 },
                new Course { CourseId = 3, Title = "Advanced EF Core", Description = null, Price = 129.99m, Level = CourseLevel.Advanced, IsPublished = true, PublishedDate = new DateTime(2025, 4, 10), InstructorId = 2, CategoryId = 2 },
                new Course { CourseId = 4, Title = "UI Design Basics", Description = "Intro to UI design", Price = 39.50m, Level = CourseLevel.Beginner, IsPublished = true, PublishedDate = new DateTime(2025, 2, 20), InstructorId = 2, CategoryId = 5 },
                new Course { CourseId = 5, Title = "Angular Deep Dive", Description = "Coming soon", Price = 149.00m, Level = CourseLevel.Advanced, IsPublished = false, PublishedDate = null, InstructorId = 3, CategoryId = 4 }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { StudentId = 1, FullName = "Sara Ahmed", Email = "sara.ahmed@example.com", DateOfBirth = new DateTime(2001, 5, 14) },
                new Student { StudentId = 2, FullName = "Omar Hassan", Email = "omar.hassan@example.com", DateOfBirth = new DateTime(2000, 11, 2) },
                new Student { StudentId = 3, FullName = "Laila Mostafa", Email = "laila.mostafa@example.com", DateOfBirth = new DateTime(2002, 8, 30) }
            );

            modelBuilder.Entity<Enrollment>().HasData(
                new Enrollment { StudentId = 1, CourseId = 1, EnrollmentDate = new DateTime(2025, 2, 1), ProgressPercentage = 40, Status = EnrollmentStatus.Active, FinalGrade = null },
                new Enrollment { StudentId = 1, CourseId = 2, EnrollmentDate = new DateTime(2025, 3, 10), ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 92.50m },
                new Enrollment { StudentId = 2, CourseId = 1, EnrollmentDate = new DateTime(2025, 2, 5), ProgressPercentage = 100, Status = EnrollmentStatus.Completed, FinalGrade = 85.00m },
                new Enrollment { StudentId = 2, CourseId = 3, EnrollmentDate = new DateTime(2025, 4, 15), ProgressPercentage = 25, Status = EnrollmentStatus.Active, FinalGrade = null },
                new Enrollment { StudentId = 3, CourseId = 1, EnrollmentDate = new DateTime(2025, 2, 8), ProgressPercentage = 10, Status = EnrollmentStatus.Dropped, FinalGrade = null },
                new Enrollment { StudentId = 3, CourseId = 4, EnrollmentDate = new DateTime(2025, 3, 1), ProgressPercentage = 70, Status = EnrollmentStatus.Active, FinalGrade = null }
            );

            base.OnModelCreating(modelBuilder);

        }
    }
}
    

