using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore; // ToQueryString, GetConnectionString
using WorkingWithEFCore.Models; //Northwind

string server = @".\net7book";

// to use SQL authentication
string username = "sa";

Write("Enter your SQL Server password:");
string? password = ReadLine();
if (string.IsNullOrWhiteSpace(password))
{
    WriteLine("Passord cannot be empty or null!");
    return;
}

string connectionString =
    $"Server={server};" +
    "Initial Catalog=Northwind;" +
    // to use SQL authentication
    "Persist Security Info=False;" +
    $"User ID={username};" +
    $"Password={password};" +
     // to use Windows authentication
     "Integrated Security=True;" +
    // other options
    "MultipleActiveResultSets=True;" +
    "Encrypt=True;" +
    "TrustServerCertificate=True;" +
    "Connection Timeout=5;";

DbContextOptionsBuilder<Northwind> builder = new();
builder.UseSqlServer(connectionString);
using (Northwind db = new(builder.Options))
{
    Write("Enter a unit price: ");
    string? priceText = ReadLine();
    if (!decimal.TryParse(priceText, out decimal price))
    {
        WriteLine("You must enter a valid unit price.");
        return;
    }

    var products = db.Products
        .Where(p => p.UnitPrice > price)
        .Select(p => new { p.ProductId, p.ProductName, p.UnitPrice });
    WriteLine("-------------------------------------------------------------");
    WriteLine("| {0,5} | {1,-35} | {2,8} |", "Id", "Name", "Price");
    WriteLine("-------------------------------------------------------------");
    foreach (var p in products)
    {
        WriteLine("| {0,5} | {1,-35} | {2,8:C} |",
            p.ProductId, p.ProductName, p.UnitPrice);
    }
    WriteLine("-------------------------------------------------------------");
    WriteLine(products.ToQueryString());
    WriteLine();
    WriteLine($"Provider: {db.Database.ProviderName}");
    WriteLine($"Connection: {db.Database.GetConnectionString()}");

}