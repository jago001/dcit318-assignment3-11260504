using System;
using System.Collections.Generic;
using System.IO;

// Student class to hold individual student data
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
        return Score switch
        {
            >= 80 and <= 100 => "A",
            >= 70 and <= 79 => "B",
            >= 60 and <= 69 => "C",
            >= 50 and <= 59 => "D",
            _ => "F"
        };
    }

    public override string ToString()
    {
        return $"{FullName} (ID: {Id}): Score = {Score}, Grade = {GetGrade()}";
    }
}

// Custom exceptions
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class CustomMissingFieldException : Exception
{
    public CustomMissingFieldException(string message) : base(message) { }
}

// Student result processor
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
                if (string.IsNullOrWhiteSpace(line)) continue;

                try
                {
                    var fields = line.Split(',', StringSplitOptions.TrimEntries);

                    if (fields.Length != 3)
                    {
                        throw new CustomMissingFieldException(
                            $"Line {lineNumber}: Expected 3 fields (ID, Name, Score), but found {fields.Length}");
                    }

                    if (!int.TryParse(fields[0], out int id))
                    {
                        throw new InvalidScoreFormatException(
                            $"Line {lineNumber}: Invalid ID format: {fields[0]}");
                    }

                    if (!int.TryParse(fields[2], out int score))
                    {
                        throw new InvalidScoreFormatException(
                            $"Line {lineNumber}: Invalid score format: {fields[2]}");
                    }

                    if (score < 0 || score > 100)
                    {
                        throw new InvalidScoreFormatException(
                            $"Line {lineNumber}: Score must be between 0 and 100: {score}");
                    }

                    students.Add(new Student(id, fields[1], score));
                }
                catch (Exception ex) when (ex is InvalidScoreFormatException || ex is CustomMissingFieldException)
                {
                    // Re-throw these specific exceptions to be handled by the main program
                    throw;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error processing line {lineNumber}: {ex.Message}");
                }
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using var writer = new StreamWriter(outputFilePath);
        
        writer.WriteLine("Student Grade Report");
        writer.WriteLine("===================");
        writer.WriteLine();

        foreach (var student in students)
        {
            writer.WriteLine(student.ToString());
        }

        writer.WriteLine();
        writer.WriteLine($"Total Students Processed: {students.Count}");
        writer.WriteLine($"Report Generated: {DateTime.Now}");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("School Grading System\n");

        var processor = new StudentResultProcessor();
        var inputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "students.txt");
        var outputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "grades_report.txt");

        try
        {
            // Create a sample input file if it doesn't exist
            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine("Creating sample input file...");
                using (var writer = new StreamWriter(inputFilePath))
                {
                    writer.WriteLine("101,John Smith,85");
                    writer.WriteLine("102,Alice Johnson,92");
                    writer.WriteLine("103,Bob Wilson,78");
                    writer.WriteLine("104,Carol Brown,65");
                    writer.WriteLine("105,David Lee,45");
                }
                Console.WriteLine($"Sample input file created at: {inputFilePath}");
            }

            Console.WriteLine("Reading student data...");
            var students = processor.ReadStudentsFromFile(inputFilePath);

            Console.WriteLine("Generating report...");
            processor.WriteReportToFile(students, outputFilePath);

            Console.WriteLine($"\nSuccess! Report generated at: {outputFilePath}");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: Input file not found at {inputFilePath}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Error: Invalid score format - {ex.Message}");
        }
        catch (CustomMissingFieldException ex)
        {
            Console.WriteLine($"Error: Missing fields - {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}
