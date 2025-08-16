using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// Marker interface for inventory entities
public interface IInventoryEntity
{
    int Id { get; }
}

// Immutable inventory record
public record InventoryItem(
    int Id,
    string Name,
    int Quantity,
    DateTime DateAdded) : IInventoryEntity;

// Generic inventory logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log;
    private readonly string _filePath;

    public InventoryLogger(string filePath)
    {
        _log = new List<T>();
        _filePath = filePath;
    }

    public void Add(T item)
    {
        if (_log.Exists(x => x.Id == item.Id))
        {
            throw new InvalidOperationException($"Item with ID {item.Id} already exists in the log.");
        }
        _log.Add(item);
    }

    public List<T> GetAll() => _log.ToList();

    public void SaveToFile()
    {
        try
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var writer = new StreamWriter(_filePath);
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(_log, options);
            writer.Write(json);
            Console.WriteLine($"Data successfully saved to {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
            throw;
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("No existing data file found. Starting with empty log.");
                _log = new List<T>();
                return;
            }

            using var reader = new StreamReader(_filePath);
            var json = reader.ReadToEnd();
            _log = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            Console.WriteLine($"Data successfully loaded from {_filePath}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error parsing JSON data: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
            throw;
        }
    }

    public void Clear()
    {
        _log.Clear();
    }
}

// Integration layer
public class InventoryApp
{
    private readonly InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string dataFilePath)
    {
        _logger = new InventoryLogger<InventoryItem>(dataFilePath);
    }

    public void SeedSampleData()
    {
        try
        {
            var sampleItems = new[]
            {
                new InventoryItem(1, "Laptop", 10, DateTime.Now),
                new InventoryItem(2, "Mouse", 20, DateTime.Now),
                new InventoryItem(3, "Keyboard", 15, DateTime.Now),
                new InventoryItem(4, "Monitor", 8, DateTime.Now),
                new InventoryItem(5, "Headphones", 25, DateTime.Now)
            };

            foreach (var item in sampleItems)
            {
                _logger.Add(item);
            }
            Console.WriteLine("Sample data seeded successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding sample data: {ex.Message}");
            throw;
        }
    }

    public void SaveData()
    {
        try
        {
            _logger.SaveToFile();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save data: {ex.Message}");
            throw;
        }
    }

    public void LoadData()
    {
        try
        {
            _logger.LoadFromFile();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load data: {ex.Message}");
            throw;
        }
    }

    public void PrintAllItems()
    {
        var items = _logger.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("No items in inventory");
            return;
        }

        Console.WriteLine("\nCurrent Inventory Items:");
        Console.WriteLine("------------------------");
        foreach (var item in items)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, " +
                            $"Quantity: {item.Quantity}, Added: {item.DateAdded:d}");
        }
        Console.WriteLine($"\nTotal Items: {items.Count}");
    }

    public void ClearMemory()
    {
        _logger.Clear();
        Console.WriteLine("Memory cleared - simulating new session");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Inventory Management System\n");

        try
        {
            // Define the data file path
            var dataFilePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Data",
                "inventory.json"
            );

            // Create and run the inventory app
            var app = new InventoryApp(dataFilePath);

            // Initial data seeding and saving
            Console.WriteLine("Seeding and saving initial data...");
            app.SeedSampleData();
            app.SaveData();
            app.PrintAllItems();

            // Simulate new session
            Console.WriteLine("\nSimulating new session...");
            app.ClearMemory();
            app.PrintAllItems();

            // Load data back
            Console.WriteLine("\nLoading data from file...");
            app.LoadData();
            app.PrintAllItems();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nA critical error occurred: {ex.Message}");
            Console.WriteLine("Please check the logs and try again.");
        }
    }
}
