using LearningWebsite.Models;

namespace LearningWebsite.Data
{
    public static class QuestionDataInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Check if questions already exist
            if (context.Questions.Any())
            {
                return;
            }

            // Get learning IDs (assuming learnings are already seeded)
            var learnings = context.Learnings.ToList();

            if (!learnings.Any())
            {
                return;
            }

            var questions = new List<Question>();

            // Add sample questions for each learning
            foreach (var learning in learnings)
            {
                // Add specific questions for Dot Net Full Stack Certification
                if (learning.Title == "Dot Net Full Stack")
                {
                    // CompleteOcean Assessment - DotNet Full Stack Beginner Level
                    questions.AddRange(new[]
                    {
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "What does CLR stand for in .NET?",
                            OptionA = "Common Language Runtime",
                            OptionB = "Core Language Repository",
                            OptionC = "Common Library Resources",
                            OptionD = "Core Language Runtime",
                            CorrectAnswer = "A",
                            DifficultyLevel = "Beginner"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "Which of the following is a value type in C#?",
                            OptionA = "String",
                            OptionB = "Integer",
                            OptionC = "Array",
                            OptionD = "Class",
                            CorrectAnswer = "B",
                            DifficultyLevel = "Beginner"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "What is the default access modifier for a class member in C#?",
                            OptionA = "Public",
                            OptionB = "Protected",
                            OptionC = "Private",
                            OptionD = "Internal",
                            CorrectAnswer = "C",
                            DifficultyLevel = "Beginner"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "Which keyword is used to inherit a class in C#?",
                            OptionA = "extends",
                            OptionB = "inherits",
                            OptionC = "implements",
                            OptionD = ":",
                            CorrectAnswer = "D",
                            DifficultyLevel = "Beginner"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "What is the purpose of the 'using' statement in C#?",
                            OptionA = "To import namespaces",
                            OptionB = "To create objects",
                            OptionC = "To define variables",
                            OptionD = "To write comments",
                            CorrectAnswer = "A",
                            DifficultyLevel = "Beginner"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "Which collection type guarantees unique elements in C#?",
                            OptionA = "List",
                            OptionB = "Array",
                            OptionC = "HashSet",
                            OptionD = "Dictionary",
                            CorrectAnswer = "C",
                            DifficultyLevel = "Beginner"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "What does MVC stand for?",
                            OptionA = "Model View Component",
                            OptionB = "Model View Controller",
                            OptionC = "Multiple View Controller",
                            OptionD = "Model Verification Controller",
                            CorrectAnswer = "B",
                            DifficultyLevel = "Beginner"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "Which method is the entry point for a C# console application?",
                            OptionA = "Start()",
                            OptionB = "Main()",
                            OptionC = "Run()",
                            OptionD = "Execute()",
                            CorrectAnswer = "B",
                            DifficultyLevel = "Beginner"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "What is the correct way to declare a constant in C#?",
                            OptionA = "constant int x = 10;",
                            OptionB = "const int x = 10;",
                            OptionC = "int const x = 10;",
                            OptionD = "final int x = 10;",
                            CorrectAnswer = "B",
                            DifficultyLevel = "Beginner"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "Which operator is used for string concatenation in C#?",
                            OptionA = "&",
                            OptionB = "+",
                            OptionC = ".",
                            OptionD = ",",
                            CorrectAnswer = "B",
                            DifficultyLevel = "Beginner"
                        },
                        // CompleteOcean Assessment - DotNet Full Stack Intermediate Level
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "What is the purpose of Entity Framework Core in .NET applications?",
                            OptionA = "To manage database migrations only",
                            OptionB = "To provide ORM capabilities for database operations",
                            OptionC = "To create database schemas",
                            OptionD = "To optimize SQL queries",
                            CorrectAnswer = "B",
                            DifficultyLevel = "Intermediate"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "Which design pattern is commonly used for dependency injection in ASP.NET Core?",
                            OptionA = "Factory Pattern",
                            OptionB = "Singleton Pattern",
                            OptionC = "Inversion of Control (IoC)",
                            OptionD = "Observer Pattern",
                            CorrectAnswer = "C",
                            DifficultyLevel = "Intermediate"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "What is the difference between IEnumerable and IQueryable in LINQ?",
                            OptionA = "IEnumerable executes queries in-memory, IQueryable executes on the database",
                            OptionB = "They are the same",
                            OptionC = "IQueryable is faster than IEnumerable",
                            OptionD = "IEnumerable supports filtering, IQueryable does not",
                            CorrectAnswer = "A",
                            DifficultyLevel = "Intermediate"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "What is middleware in ASP.NET Core?",
                            OptionA = "A database connection layer",
                            OptionB = "Software components that handle HTTP requests and responses",
                            OptionC = "A caching mechanism",
                            OptionD = "A logging framework",
                            CorrectAnswer = "B",
                            DifficultyLevel = "Intermediate"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "Which HTTP method is idempotent and safe?",
                            OptionA = "POST",
                            OptionB = "PUT",
                            OptionC = "DELETE",
                            OptionD = "GET",
                            CorrectAnswer = "D",
                            DifficultyLevel = "Intermediate"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "What is the purpose of the async/await pattern in C#?",
                            OptionA = "To run code in parallel",
                            OptionB = "To write asynchronous code that is easier to read and maintain",
                            OptionC = "To improve memory usage",
                            OptionD = "To create background threads",
                            CorrectAnswer = "B",
                            DifficultyLevel = "Intermediate"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "In ASP.NET Core, what is the difference between AddScoped, AddTransient, and AddSingleton?",
                            OptionA = "They control the lifetime of dependency injection services",
                            OptionB = "They define the scope of variables",
                            OptionC = "They manage database connections",
                            OptionD = "They configure middleware",
                            CorrectAnswer = "A",
                            DifficultyLevel = "Intermediate"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "What is the purpose of DTOs (Data Transfer Objects)?",
                            OptionA = "To transfer data between layers without exposing domain models",
                            OptionB = "To store data in the database",
                            OptionC = "To validate user input",
                            OptionD = "To log application errors",
                            CorrectAnswer = "A",
                            DifficultyLevel = "Intermediate"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "Which attribute is used to specify routing in ASP.NET Core Web API?",
                            OptionA = "[HttpRoute]",
                            OptionB = "[Route]",
                            OptionC = "[ApiRoute]",
                            OptionD = "[Path]",
                            CorrectAnswer = "B",
                            DifficultyLevel = "Intermediate"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = "What is the purpose of CORS (Cross-Origin Resource Sharing) in web applications?",
                            OptionA = "To encrypt data transmission",
                            OptionB = "To allow or restrict resources from different domains",
                            OptionC = "To compress HTTP responses",
                            OptionD = "To manage session state",
                            CorrectAnswer = "B",
                            DifficultyLevel = "Intermediate"
                        }
                    });
                }
                // Add 15 questions per learning topic
                else if (learning.Category == "Technical")
                {
                    questions.AddRange(new[]
                    {
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What is the primary purpose of {learning.Title}?",
                            OptionA = "To improve performance",
                            OptionB = "To enhance security",
                            OptionC = "To simplify development",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"Which of the following is a key feature of {learning.Title}?",
                            OptionA = "Cross-platform compatibility",
                            OptionB = "Built-in dependency injection",
                            OptionC = "Integrated testing framework",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What programming language is primarily used in {learning.Title}?",
                            OptionA = "Java",
                            OptionB = "C#",
                            OptionC = "Python",
                            OptionD = "JavaScript",
                            CorrectAnswer = "B"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"How does {learning.Title} improve application performance?",
                            OptionA = "Through caching",
                            OptionB = "Through asynchronous processing",
                            OptionC = "Through optimized algorithms",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What is the recommended way to handle errors in {learning.Title}?",
                            OptionA = "Try-catch blocks",
                            OptionB = "Global exception handlers",
                            OptionC = "Logging frameworks",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"Which design pattern is commonly used in {learning.Title}?",
                            OptionA = "MVC Pattern",
                            OptionB = "Repository Pattern",
                            OptionC = "Factory Pattern",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What security feature does {learning.Title} provide?",
                            OptionA = "Authentication",
                            OptionB = "Authorization",
                            OptionC = "Data encryption",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"How can you test {learning.Title} applications?",
                            OptionA = "Unit testing",
                            OptionB = "Integration testing",
                            OptionC = "End-to-end testing",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What is the best practice for deploying {learning.Title} applications?",
                            OptionA = "Using containers",
                            OptionB = "Cloud hosting",
                            OptionC = "Continuous integration/deployment",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"Which database type works best with {learning.Title}?",
                            OptionA = "SQL databases",
                            OptionB = "NoSQL databases",
                            OptionC = "In-memory databases",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What is dependency injection in {learning.Title}?",
                            OptionA = "A design pattern for loose coupling",
                            OptionB = "A way to manage object lifecycles",
                            OptionC = "A testing technique",
                            OptionD = "Both A and B",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"How do you handle configuration in {learning.Title}?",
                            OptionA = "Using JSON files",
                            OptionB = "Using environment variables",
                            OptionC = "Using Azure Key Vault",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What is middleware in {learning.Title}?",
                            OptionA = "Software that sits between OS and applications",
                            OptionB = "Components that handle HTTP requests",
                            OptionC = "Database connection pooling",
                            OptionD = "Cache management system",
                            CorrectAnswer = "B"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"Which version control system is recommended for {learning.Title} projects?",
                            OptionA = "Git",
                            OptionB = "SVN",
                            OptionC = "Mercurial",
                            OptionD = "Perforce",
                            CorrectAnswer = "A"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What is the benefit of using async/await in {learning.Title}?",
                            OptionA = "Better resource utilization",
                            OptionB = "Improved responsiveness",
                            OptionC = "Easier to write asynchronous code",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        }
                    });
                }
                else // Soft Skills
                {
                    questions.AddRange(new[]
                    {
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What is the key principle of {learning.Title}?",
                            OptionA = "Clear communication",
                            OptionB = "Active listening",
                            OptionC = "Empathy",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"How can {learning.Title} improve team productivity?",
                            OptionA = "Better collaboration",
                            OptionB = "Reduced conflicts",
                            OptionC = "Improved morale",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What is the most important aspect of {learning.Title}?",
                            OptionA = "Consistency",
                            OptionB = "Adaptability",
                            OptionC = "Practice",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"How do you demonstrate {learning.Title}?",
                            OptionA = "Through actions",
                            OptionB = "Through words",
                            OptionC = "Through results",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What challenge might you face when learning {learning.Title}?",
                            OptionA = "Resistance to change",
                            OptionB = "Lack of practice",
                            OptionC = "Cultural differences",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"Why is {learning.Title} important in the workplace?",
                            OptionA = "Builds trust",
                            OptionB = "Enhances teamwork",
                            OptionC = "Improves outcomes",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"How can you improve your {learning.Title} skills?",
                            OptionA = "Training and workshops",
                            OptionB = "Feedback from others",
                            OptionC = "Self-reflection",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What role does feedback play in {learning.Title}?",
                            OptionA = "Identifies areas for improvement",
                            OptionB = "Reinforces good practices",
                            OptionC = "Builds confidence",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"How does {learning.Title} contribute to career growth?",
                            OptionA = "Opens new opportunities",
                            OptionB = "Builds professional network",
                            OptionC = "Enhances leadership potential",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What is a common barrier to effective {learning.Title}?",
                            OptionA = "Poor communication",
                            OptionB = "Lack of awareness",
                            OptionC = "Fixed mindset",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"How can leaders demonstrate {learning.Title}?",
                            OptionA = "Leading by example",
                            OptionB = "Encouraging team members",
                            OptionC = "Providing support",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What impact does {learning.Title} have on team dynamics?",
                            OptionA = "Improves collaboration",
                            OptionB = "Reduces conflicts",
                            OptionC = "Increases engagement",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"How do you measure success in {learning.Title}?",
                            OptionA = "Behavioral changes",
                            OptionB = "Improved relationships",
                            OptionC = "Positive outcomes",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"What resources support {learning.Title} development?",
                            OptionA = "Books and articles",
                            OptionB = "Mentorship programs",
                            OptionC = "Online courses",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        },
                        new Question
                        {
                            LearningId = learning.Id,
                            QuestionText = $"How does {learning.Title} relate to emotional intelligence?",
                            OptionA = "Requires self-awareness",
                            OptionB = "Involves empathy",
                            OptionC = "Includes social skills",
                            OptionD = "All of the above",
                            CorrectAnswer = "D"
                        }
                    });
                }
            }

            context.Questions.AddRange(questions);
            context.SaveChanges();
        }
    }
}
