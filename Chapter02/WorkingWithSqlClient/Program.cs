using Microsoft.Data.SqlClient;

string server = @".\net7bookz";

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
    "Connection Timeout=30;";
SqlConnection connection = new SqlConnection(connectionString);
connection.StateChange += Connection_StateChange;
connection.InfoMessage += Connection_InfoMessage;

try
{
    connection.Open();
    WriteLine($"SQLServer version: {connection.ServerVersion}");
}

catch (SqlException ex)
{
    WriteLine($"SQL exception: {ex.Message}");
    return;
}

connection.Close();
