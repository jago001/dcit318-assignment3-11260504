using System;
using System.Collections.Generic;

// Marker Interface
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// Product Classes
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString()
    {
        return $"Electronic - ID: {Id}, Name: {Name}, Brand: {Brand}, Quantity: {Quantity}, Warranty: {WarrantyMonths} months";
    }
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString()
    {
        return $"Grocery - ID: {Id}, Name: {Name}, Quantity: {Quantity}, Expires: {ExpiryDate:d}";
    }
}

// Custom Exceptions
public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// Generic Inventory Repository
public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException($"Item with ID {item.Id} already exists in inventory.");
        }
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.TryGetValue(id, out T? item))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found in inventory.");
        }
        return item;
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found in inventory.");
        }
    }

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException($"Invalid quantity: {newQuantity}. Quantity must be non-negative.");
        }

        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

// Warehouse Manager
public class WarehouseManager
{
    private readonly InventoryRepository<ElectronicItem> _electronics = new();
    private readonly InventoryRepository<GroceryItem> _groceries = new();

    public void SeedData()
    {
        // Add electronic items
        try
        {
            _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
            _electronics.AddItem(new ElectronicItem(2, "Smartphone", 15, "Samsung", 12));
            _electronics.AddItem(new ElectronicItem(3, "Tablet", 8, "Apple", 12));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Error seeding electronic items: {ex.Message}");
        }

        // Add grocery items
        try
        {
            _groceries.AddItem(new GroceryItem(1, "Milk", 50, DateTime.Now.AddDays(7)));
            _groceries.AddItem(new GroceryItem(2, "Bread", 30, DateTime.Now.AddDays(5)));
            _groceries.AddItem(new GroceryItem(3, "Eggs", 100, DateTime.Now.AddDays(14)));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Error seeding grocery items: {ex.Message}");
        }
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        var items = repo.GetAllItems();
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Successfully updated quantity for item {id}. New quantity: {item.Quantity}");
        }
        catch (Exception ex) when (ex is ItemNotFoundException || ex is InvalidQuantityException)
        {
            Console.WriteLine($"Error increasing stock: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Successfully removed item {id}");
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }

    // Additional methods to demonstrate exception handling
    public void DemonstrateExceptions()
    {
        Console.WriteLine("\nDemonstrating Exception Handling:");

        // Try to add a duplicate item
        try
        {
            Console.WriteLine("\nTrying to add duplicate electronic item...");
            _electronics.AddItem(new ElectronicItem(1, "Duplicate Laptop", 5, "Dell", 24));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Expected error: {ex.Message}");
        }

        // Try to remove non-existent item
        try
        {
            Console.WriteLine("\nTrying to remove non-existent item...");
            _electronics.RemoveItem(999);
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"Expected error: {ex.Message}");
        }

        // Try to update with invalid quantity
        try
        {
            Console.WriteLine("\nTrying to update with invalid quantity...");
            _electronics.UpdateQuantity(1, -5);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Expected error: {ex.Message}");
        }
    }

    public InventoryRepository<ElectronicItem> Electronics => _electronics;
    public InventoryRepository<GroceryItem> Groceries => _groceries;
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Warehouse Inventory Management System\n");

        var warehouse = new WarehouseManager();
        
        // Seed initial data
        warehouse.SeedData();

        // Print all items
        Console.WriteLine("\nGrocery Items:");
        warehouse.PrintAllItems(warehouse.Groceries);

        Console.WriteLine("\nElectronic Items:");
        warehouse.PrintAllItems(warehouse.Electronics);

        // Demonstrate exception handling
        warehouse.DemonstrateExceptions();

        // Demonstrate successful operations
        Console.WriteLine("\nDemonstrating successful operations:");
        warehouse.IncreaseStock(warehouse.Electronics, 1, 5);
        warehouse.RemoveItemById(warehouse.Groceries, 1);
    }
}
