using System;
using System.Collections.Generic;

namespace AdvCSharpExam
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose the type of exam:\n1. Practical Exam\n2. Final Exam");
            int examType = Utility.GetValidIntInput("Enter your choice (1 or 2): ", 1, 2);

            int examDuration = Utility.GetValidIntInput("Enter the duration of the exam in minutes: ", 1, 300);
            int totalQuestions = Utility.GetValidIntInput("Enter the total number of questions: ", 1, 50);

            Console.Write("Enter Subject Name: ");
            string subjectName = Console.ReadLine();
            Subject subject = new Subject(subjectName);

            Exam exam = examType == 1 ? new PracticalExam() : new FinalExam();
            exam.Duration = examDuration;
            subject.Exam = exam;

            for (int i = 1; i <= totalQuestions; i++)
            {
                Console.WriteLine($"\nCreating Question {i}:");
                Console.Write("Enter question header: ");
                string header = Console.ReadLine();

                if (exam is FinalExam)
                {
                    int questionType = Utility.GetValidIntInput("Choose question type:\n1. True or False\n2. MCQ\nEnter choice: ", 1, 2);

                    if (questionType == 1)
                    {
                        Console.Write("Enter the True or False question: ");
                        string body = Console.ReadLine();

                        int mark = Utility.GetValidIntInput("Enter the mark for this question: ", 1, 10);
                        bool correctAnswer = Utility.GetValidIntInput("Enter the correct answer (1 for True, 2 for False): ", 1, 2) == 1;

                        exam.AddQuestion(new TrueFalseQuestion(header, body, mark, correctAnswer));
                    }
                    else
                    {
                        exam.AddQuestion(CreateMCQ(header));
                    }
                }
                else
                {
                    exam.AddQuestion(CreateMCQ(header));
                }
            }

            Console.WriteLine("Would you like to take the exam? (y/n)");
            if (Console.ReadLine().ToLower() == "y")
            {
                exam.TakeExam();
            }
            else
            {
                Console.WriteLine("No Exam");
            }
        }

        static MCQQuestion CreateMCQ(string header)
        {
            Console.Write("Enter the MCQ question body: ");
            string body = Console.ReadLine();

            int mark = Utility.GetValidIntInput("Enter the mark for this question: ", 1, 10);
            MCQQuestion mcqQuestion = new MCQQuestion(header, body, mark);

            Answer[] answers = new Answer[3];
            for (int j = 0; j < 3; j++)
            {
                Console.Write($"Enter choice {j + 1}: ");
                answers[j] = new Answer(j + 1, Console.ReadLine());
            }
            mcqQuestion.SetChoices(answers);
            mcqQuestion.SetCorrectAnswer(Utility.GetValidIntInput("Enter the number of the correct choice (1, 2, or 3): ", 1, 3) - 1);

            return mcqQuestion;
        }
    }

    static class Utility
    {
        public static int GetValidIntInput(string message, int min, int max)
        {
            int result;
            do
            {
                Console.Write(message);
            } while (!int.TryParse(Console.ReadLine(), out result) || result < min || result > max);
            return result;
        }
    }

    class Subject
    {
        public int SubjectId { get; }
        public string SubjectName { get; }
        public Exam Exam { get; set; }

        public Subject(string name)
        {
            SubjectId = new Random().Next(1, 1000);
            SubjectName = name;
        }
    }

    class Answer
    {
        public int Id { get; }
        public string Text { get; }
        public Answer(int id, string text)
        {
            Id = id;
            Text = text;
        }
    }

    abstract class Exam : ICloneable, IComparable<Exam>
    {
        public int Duration { get; set; }
        public List<Question> Questions { get; } = new List<Question>();

        public void AddQuestion(Question question) => Questions.Add(question);

        public abstract void TakeExam();

        public int CompareTo(Exam other) => this.Duration.CompareTo(other.Duration);
        public object Clone() => this.MemberwiseClone();
    }

    class PracticalExam : Exam
    {
        public override void TakeExam()
        {
            Console.WriteLine("\nPractical Exam Completed! Here are the correct answers:");
            foreach (var question in Questions)
            {
                Console.WriteLine($"{question.Header}: {question.GetCorrectAnswer()}");
            }
        }
    }

    class FinalExam : Exam
    {
        public override void TakeExam()
        {
            Console.WriteLine("\nFinal Exam Results:");
            int totalScore = 0, earnedScore = 0;
            foreach (var question in Questions)
            {
                Console.WriteLine($"{question.Header}: {question.Text}");
                earnedScore += question.Ask();
                totalScore += question.Mark;
            }
            Console.WriteLine($"Your Score: {earnedScore}/{totalScore}");
        }
    }

    abstract class Question
    {
        public string Header { get; }
        public string Text { get; }
        public int Mark { get; }

        protected Question(string header, string text, int mark)
        {
            Header = header;
            Text = text;
            Mark = mark;
        }

        public abstract int Ask();
        public abstract string GetCorrectAnswer();
    }

    class TrueFalseQuestion : Question
    {
        public bool CorrectAnswer { get; }

        public TrueFalseQuestion(string header, string text, int mark, bool correctAnswer) : base(header, text, mark)
        {
            CorrectAnswer = correctAnswer;
        }

        public override int Ask() => Utility.GetValidIntInput("Your answer (1 for True, 2 for False): ", 1, 2) == (CorrectAnswer ? 1 : 2) ? Mark : 0;
        public override string GetCorrectAnswer() => CorrectAnswer ? "True" : "False";
    }

    class MCQQuestion : Question
    {
        private Answer[] Choices;
        private int CorrectAnswerIndex;

        public MCQQuestion(string header, string text, int mark) : base(header, text, mark) { }

        public void SetChoices(Answer[] choices) => Choices = choices;
        public void SetCorrectAnswer(int index) => CorrectAnswerIndex = index;

        public override int Ask() => Utility.GetValidIntInput("Your answer: ", 1, Choices.Length) - 1 == CorrectAnswerIndex ? Mark : 0;
        public override string GetCorrectAnswer() => Choices[CorrectAnswerIndex].Text;
    }
}
