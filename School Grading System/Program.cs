using System;
using System.Collections.Generic;
using System.IO;


public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}


public class Student
{
    public int Id { get; }
    public string FullName { get; }
    public int Score { get; }

    public Student(int id, string fullName, int score)
    {
        Id = id;
        FullName = fullName;
        Score = score;
    }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70 && Score <= 79) return "B";
        if (Score >= 60 && Score <= 69) return "C";
        if (Score >= 50 && Score <= 59) return "D";
        return "F";
    }

    public override string ToString()
    {
        return $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
    }
}


public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        var students = new List<Student>();

        using (var reader = new StreamReader(inputFilePath))
        {
            string? line;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;
                var parts = line.Split(',');

                if (parts.Length != 3)
                {
                    throw new MissingFieldException($"Line {lineNumber}: Missing field(s).");
                }

                if (!int.TryParse(parts[0], out int id))
                {
                    throw new FormatException($"Line {lineNumber}: Invalid ID format.");
                }

                string name = parts[1].Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new MissingFieldException($"Line {lineNumber}: Name is missing.");
                }

                if (!int.TryParse(parts[2], out int score))
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score format.");
                }

                students.Add(new Student(id, name, score));
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (var writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                writer.WriteLine(student);
            }
        }
    }
}


class Program
{
    static void Main()
    {
        var processor = new StudentResultProcessor();

        Console.WriteLine("Enter input file path (e.g., students.txt):");
        string inputPath = Console.ReadLine();

        Console.WriteLine("Enter output file path (e.g., report.txt):");
        string outputPath = Console.ReadLine();

        try
        {
            var students = processor.ReadStudentsFromFile(inputPath);
            processor.WriteReportToFile(students, outputPath);
            Console.WriteLine($"Report generated successfully at {outputPath}");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: The input file was not found.");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Score format error: {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Missing field error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
