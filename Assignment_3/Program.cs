using Assignment_3.Contexts;
using Assignment_3.Enums;
using Assignment_3.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment_3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Part 02: Operations

            #region Create

            Console.WriteLine("===== CREATE =====");

            Console.WriteLine("\n--- 1. Add an instructor ---");
            CreateInstructor("Khaled Mostafa", "khaled.mostafa@example.com", "Data engineer", DateTime.Now);
            CreateInstructor("", "no.name@example.com", null, DateTime.Now);   // rejected

            Console.WriteLine("\n--- 2. Add a course ---");
            CreateCourse("Python for Data Science", "Intro to Python", 79.99m, CourseLevel.Beginner, instructorId: 1, categoryId: 5);  // success
            CreateCourse("Ghost Course", null, 10m, CourseLevel.Beginner, instructorId: 99, categoryId: 6);                            // rejected
            CreateCourse("Ghost Course", null, 10m, CourseLevel.Beginner, instructorId: 1, categoryId: 99);                            // rejected
            CreateCourse("Bad Price", null, -5m, CourseLevel.Beginner, instructorId: 1, categoryId: 6);                                // rejected

            Console.WriteLine("\n--- 3. Add several students ---");
            CreateStudents();

            Console.WriteLine("\n--- 4. Enroll a student ---");
            CreateEnrollment(studentId: 3, courseId: 2);    // success
            CreateEnrollment(studentId: 1, courseId: 5);    // rejected
            CreateEnrollment(studentId: 1, courseId: 1);    // rejected
            CreateEnrollment(studentId: 99, courseId: 1);   // rejected
            CreateEnrollment(studentId: 1, courseId: 99);   // rejected

            #endregion

            #region Read

            Console.WriteLine("\n===== READ =====");

            Console.WriteLine("\n--- 1. Find a course ---");
            FindCourse(courseId: 1, title: "C# Fundamentals");   // found
            FindCourse(courseId: 99, title: "No Such Course");   // not found

            Console.WriteLine("\n--- 2. Filter courses by price ---");
            FilterCoursesByPrice(40m, 130m);
            FilterCoursesByPrice(500m, 600m);   // no results

            Console.WriteLine("\n--- 3. Sub-categories ---");
            PrintSubCategories(categoryId: 1);    // has sub-categories
            PrintSubCategories(categoryId: 6);    // no sub-categories
            PrintSubCategories(categoryId: 99);   // not found
            PrintTopLevelCategories();

            Console.WriteLine("\n--- 4. Course roster ---");
            PrintCourseRoster(courseId: 1);
            PrintCourseRoster(courseId: 5);    // no students
            PrintCourseRoster(courseId: 99);   // not found

            #endregion

            #region Update

            Console.WriteLine("\n===== UPDATE =====");

            Console.WriteLine("\n--- 1. Change a price ---");
            ChangeCoursePrice(courseId: 1, newPrice: 59.99m);    // success
            ChangeCoursePrice(courseId: 1, newPrice: -10m);      // rejected
            ChangeCoursePrice(courseId: 99, newPrice: 20m);      // rejected

            Console.WriteLine("\n--- 2. Update progress ---");
            UpdateProgress(studentId: 1, courseId: 1, newProgress: 75);    // success
            UpdateProgress(studentId: 1, courseId: 1, newProgress: 100);   // success
            UpdateProgress(studentId: 2, courseId: 3, newProgress: 150);   // rejected
            UpdateProgress(studentId: 2, courseId: 3, newProgress: -5);    // rejected
            UpdateProgress(studentId: 99, courseId: 1, newProgress: 50);   // rejected

            Console.WriteLine("\n--- 3. Publish a course ---");
            PublishCourse(courseId: 5);    // success
            PublishCourse(courseId: 5);    // rejected
            PublishCourse(courseId: 99);   // rejected

            #endregion

            #region Delete

            Console.WriteLine("\n===== DELETE =====");

            Console.WriteLine("\n--- 1. Unenroll ---");
            Unenroll(studentId: 3, courseId: 4);    // success
            Unenroll(studentId: 3, courseId: 4);    // rejected
            Unenroll(studentId: 99, courseId: 1);   // rejected

            Console.WriteLine("\n--- 2. Delete a category ---");
            DeleteCategoryUnsafe(categoryId: 1);    // fails
            DeleteCategorySafe(categoryId: 1);      // refused 
            DeleteCategorySafe(categoryId: 6);      // success
            DeleteCategorySafe(categoryId: 99);     // rejected

            Console.WriteLine("\n--- 3. Delete a course ---");
            DeleteCourse(courseId: 1);    // success
            DeleteCourse(courseId: 99);   // rejected

            #endregion



            #endregion
        }

        #region Create Operations

        #region Add Instructor

        static void CreateInstructor(string fullName, string email, string? bio, DateTime joinDate)
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            if (string.IsNullOrWhiteSpace(fullName))
            {
                Console.WriteLine("Cannot add instructor: invalid name.");
                return;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Cannot add instructor: invalid email.");
                return;
            }

            Instructor instructor = new Instructor
            {
                FullName = fullName,
                Email = email,
                Bio = bio,
                JoinDate = joinDate
            };

            context.Instructors.Add(instructor);
            context.SaveChanges();

            Console.WriteLine($"Instructor added successfully. ID: {instructor.InstructorId}");
        }

        #endregion

        #region Add Course

        static void CreateCourse(string title, string? description, decimal price, CourseLevel level, int instructorId, int categoryId)
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Cannot add course: invalid title.");
                return;
            }

            if (price < 0)
            {
                Console.WriteLine("Cannot add course: price cannot be negative.");
                return;
            }

            if (!Enum.IsDefined(typeof(CourseLevel), level))
            {
                Console.WriteLine("Cannot add course: invalid level.");
                return;
            }

            // Check the instructor exists
            var instructor = context.Instructors.Find(instructorId);
            if (instructor == null)
            {
                Console.WriteLine($"Cannot add course: instructor {instructorId} not found.");
                return;
            }

            // Check the category exists
            var category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Cannot add course: category {categoryId} not found.");
                return;
            }

            Course course = new Course
            {
                Title = title,
                Description = description,
                Price = price,
                Level = level,
                IsPublished = false,
                PublishedDate = null,
                InstructorId = instructorId,
                CategoryId = categoryId
            };

            context.Courses.Add(course);
            context.SaveChanges();

            Console.WriteLine($"Course '{course.Title}' added successfully. ID: {course.CourseId}");
        }

        #endregion

        #region Add Several Students

        static void CreateStudents()
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            List<Student> students = new List<Student>
            {
                new Student { FullName = "Youssef Ali", Email = "youssef.ali@example.com", DateOfBirth = new DateTime(2001, 3, 12) },
                new Student { FullName = "Mariam Samy", Email = "mariam.samy@example.com", DateOfBirth = new DateTime(2002, 7, 25) },
                new Student { FullName = "Karim Nabil", Email = "karim.nabil@example.com", DateOfBirth = new DateTime(2000, 12, 1) }
            };

            context.Students.AddRange(students);
            context.SaveChanges();

            Console.WriteLine("Three students added successfully.");

            foreach (var student in students)
            {
                Console.WriteLine($"Student: {student.FullName} - ID: {student.StudentId}");
            }
        }

        #endregion

        #region Enroll Student

        static void CreateEnrollment(int studentId, int courseId)
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            var student = context.Students.Find(studentId);
            if (student == null)
            {
                Console.WriteLine($"Enrollment refused: student {studentId} not found.");
                return;
            }

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Enrollment refused: course {courseId} not found.");
                return;
            }


            if (!course.IsPublished)
            {
                Console.WriteLine($"Enrollment refused: course '{course.Title}' is not published.");
                return;
            }


            bool alreadyEnrolled = context.Enrollments.Any(
                e => e.StudentId == studentId && e.CourseId == courseId);

            if (alreadyEnrolled)
            {
                Console.WriteLine($"Enrollment refused: {student.FullName} is already enrolled in '{course.Title}'.");
                return;
            }

            Enrollment enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                ProgressPercentage = 0,
                Status = EnrollmentStatus.Active,
                FinalGrade = null
            };

            context.Enrollments.Add(enrollment);
            context.SaveChanges();

            Console.WriteLine($"{student.FullName} enrolled in '{course.Title}' successfully.");
        }

        #endregion

        #endregion


        #region Read Operations

        #region Find Course

        static void FindCourse(int courseId, string title)
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            // Find 
            var byId = context.Courses.Find(courseId);
            if (byId == null)
                Console.WriteLine($"Find: course with Id {courseId} not found.");
            else
                Console.WriteLine($"Find: Id {byId.CourseId} - {byId.Title}");

            // FirstOrDefault 
            var byTitle = context.Courses.FirstOrDefault(c => c.Title == title);
            if (byTitle == null)
                Console.WriteLine($"FirstOrDefault: course titled '{title}' not found.");
            else
                Console.WriteLine($"FirstOrDefault: Id {byTitle.CourseId} - {byTitle.Title}");
        }

        #endregion

        #region Filter Courses

        static void FilterCoursesByPrice(decimal minPrice, decimal maxPrice)
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            if (minPrice > maxPrice)
            {
                Console.WriteLine("Invalid range: minimum price is greater than maximum price.");
                return;
            }


            var courses = context.Courses
                .Where(c => c.Price >= minPrice && c.Price <= maxPrice)
                .OrderByDescending(c => c.Price)
                .Select(c => new { c.Title, c.Level, c.Price })
                .ToList();

            Console.WriteLine($"Courses priced between {minPrice} and {maxPrice}:");

            if (courses.Count == 0)
            {
                Console.WriteLine("  No courses found.");
                return;
            }

            foreach (var c in courses)
                Console.WriteLine($"  {c.Title} | {c.Level} | {c.Price}");
        }

        #endregion

        #region Sub-categories

        static void PrintSubCategories(int categoryId)
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            var category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Category {categoryId} not found.");
                return;
            }


            var subCategories = context.Categories
                .Where(c => c.ParentCategoryId == categoryId)
                .ToList();

            Console.WriteLine($"Sub-categories of '{category.Name}':");

            if (subCategories.Count == 0)
            {
                Console.WriteLine("  None.");
                return;
            }

            foreach (var sub in subCategories)
                Console.WriteLine($"  {sub.CategoryId} - {sub.Name}");
        }

        static void PrintTopLevelCategories()
        {
            using LearningPlatformContext context = new LearningPlatformContext();


            var topLevel = context.Categories
                .Where(c => c.ParentCategoryId == null)
                .ToList();

            Console.WriteLine("Top-level categories:");

            foreach (var c in topLevel)
                Console.WriteLine($"  {c.CategoryId} - {c.Name}");
        }

        #endregion

        #region Course Roster

        static void PrintCourseRoster(int courseId)
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Course {courseId} not found.");
                return;
            }

            // Step 1 => get the student Ids from the enrollments
            var studentIds = context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToList();

            Console.WriteLine($"Course '{course.Title}' has {studentIds.Count} enrolled student(s).");

            if (studentIds.Count == 0)
                return;

            // Step 2 => get the students using Contains
            var students = context.Students
                .Where(s => studentIds.Contains(s.StudentId))
                .ToList();

            foreach (var s in students)
                Console.WriteLine($"  {s.FullName}");
        }

        #endregion

        #endregion


        #region Update Operations

        #region Change Price

        static void ChangeCoursePrice(int courseId, decimal newPrice)
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            if (newPrice < 0)
            {
                Console.WriteLine("Cannot change price: price cannot be negative.");
                return;
            }

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Cannot change price: course {courseId} not found.");
                return;
            }

            decimal oldPrice = course.Price;

            course.Price = newPrice;
            context.SaveChanges();

            Console.WriteLine($"Price of '{course.Title}' changed from {oldPrice} to {newPrice}.");
        }

        #endregion

        #region Update Progress

        static void UpdateProgress(int studentId, int courseId, int newProgress)
        {
            using LearningPlatformContext context = new LearningPlatformContext();


            if (newProgress < 0 || newProgress > 100)
            {
                Console.WriteLine($"Cannot update progress: {newProgress} is outside the range 0-100.");
                return;
            }

            // Composite key
            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment == null)
            {
                Console.WriteLine($"Cannot update progress: student {studentId} is not enrolled in course {courseId}.");
                return;
            }

            enrollment.ProgressPercentage = newProgress;


            if (newProgress == 100)
                enrollment.Status = EnrollmentStatus.Completed;

            context.SaveChanges();

            Console.WriteLine($"Progress updated to {newProgress}. Status: {enrollment.Status}.");
        }

        #endregion

        #region Publish Course

        static void PublishCourse(int courseId)
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Cannot publish: course {courseId} not found.");
                return;
            }

            if (course.IsPublished)
            {
                Console.WriteLine($"Course '{course.Title}' is already published.");
                return;
            }

            course.IsPublished = true;
            course.PublishedDate = DateTime.Now;
            context.SaveChanges();

            Console.WriteLine($"Course '{course.Title}' published on {course.PublishedDate:yyyy-MM-dd}.");
        }

        #endregion

        #endregion


        #region Delete Operations

        #region Unenroll

        static void Unenroll(int studentId, int courseId)
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            // Composite key
            var enrollment = context.Enrollments.Find(studentId, courseId);
            if (enrollment == null)
            {
                Console.WriteLine($"Cannot unenroll: student {studentId} is not enrolled in course {courseId}.");
                return;
            }

            context.Enrollments.Remove(enrollment);
            context.SaveChanges();

            Console.WriteLine($"Student {studentId} unenrolled from course {courseId}.");
        }

        #endregion

        #region Delete Category

        // Unsafe version
        static void DeleteCategoryUnsafe(int categoryId)
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            var category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Category {categoryId} not found.");
                return;
            }

            try
            {
                context.Categories.Remove(category);
                context.SaveChanges();
                Console.WriteLine($"Category '{category.Name}' deleted.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Unsafe delete failed: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // Safe version
        static void DeleteCategorySafe(int categoryId)
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            var category = context.Categories.Find(categoryId);
            if (category == null)
            {
                Console.WriteLine($"Cannot delete: category {categoryId} not found.");
                return;
            }

            bool hasSubCategories = context.Categories.Any(c => c.ParentCategoryId == categoryId);
            if (hasSubCategories)
            {
                Console.WriteLine($"Cannot delete '{category.Name}': it still has sub-categories.");
                return;
            }


            bool hasCourses = context.Courses.Any(c => c.CategoryId == categoryId);
            if (hasCourses)
            {
                Console.WriteLine($"Cannot delete '{category.Name}': it still has courses.");
                return;
            }

            context.Categories.Remove(category);
            context.SaveChanges();

            Console.WriteLine($"Category '{category.Name}' deleted.");
        }

        #endregion

        #region Delete Course

        static void DeleteCourse(int courseId)
        {
            using LearningPlatformContext context = new LearningPlatformContext();

            var course = context.Courses.Find(courseId);
            if (course == null)
            {
                Console.WriteLine($"Cannot delete: course {courseId} not found.");
                return;
            }

            int before = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"Enrollments of '{course.Title}' before delete: {before}");

            context.Courses.Remove(course);
            context.SaveChanges();

            int after = context.Enrollments.Count(e => e.CourseId == courseId);
            Console.WriteLine($"Enrollments of '{course.Title}' after delete: {after}");

            // The enrollments were removed automatically because the Course-Enrollment
            // relationship is configured with Cascade delete (OnDelete(DeleteBehavior.Cascade)).
            // The foreign key in SQL Server is on Delete Cascade, so the database itself
            // deletes the child rows when the course row is deleted.


        }

        #endregion

        #endregion
    }
}