using Microsoft.Data.SqlClient;
using System.Data;

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
SqlConnection connection = new SqlConnection(connectionString);
connection.StateChange += Connection_StateChange;
connection.InfoMessage += Connection_InfoMessage;

try
{
    await connection.OpenAsync();  // using OpenAsync to make it asynchronous
    WriteLine($"SQLServer version: {connection.ServerVersion}");
}

catch (SqlException ex)
{
    WriteLine($"SQL exception: {ex.Message}");
    return;
}



Write("Enter a unit price: ");

string? priceText = ReadLine();
if (!decimal.TryParse(priceText, out decimal price))
{
    WriteLine("You must enter a valid unit price");
    return;
}

SqlCommand cmd = connection.CreateCommand();
/*
 * THIS COMMENTED CODE GETS REPLACES BY THE STORED PROCEDURE.
cmd.CommandType = CommandType.Text;
cmd.CommandText = "SELECT ProductId, ProductName, UnitPrice FROM Products" + " WHERE UnitPrice > @price";

cmd.Parameters.AddWithValue("price", price);
*/

cmd.CommandType = CommandType.StoredProcedure;
cmd.CommandText = "GetExpensiveProducts";

SqlParameter p1 = new()
{
    ParameterName = "price",
    SqlDbType = SqlDbType.Money,
    SqlValue = price
};
SqlParameter p2 = new()
{
    Direction = ParameterDirection.Output,
    ParameterName = "count",
    SqlDbType = SqlDbType.Int
};
SqlParameter p3 = new()
{
    Direction = ParameterDirection.ReturnValue,
    ParameterName = "rv",
    SqlDbType = SqlDbType.Int
};
cmd.Parameters.Add(p1);
cmd.Parameters.Add(p2);
cmd.Parameters.Add(p3);


//SqlDataReader r = cmd.ExecuteReader(); Original execute command
SqlDataReader r = await cmd.ExecuteReaderAsync();  //Asyncronous version of ExecuteReader above.
WriteLine("-------------------------------------------------------------");
WriteLine("| {0,5} | {1,-35} | {2,8} |", "Id", "Name", "Price");
WriteLine("-------------------------------------------------------------");

while (await r.ReadAsync())
{
    WriteLine("| {0,5} | {1,-35} | {2,8:C} |",
        await r.GetFieldValueAsync<int>("ProductId"),
        await r.GetFieldValueAsync<string>("ProductName"),
        await r.GetFieldValueAsync<decimal>("UnitPrice"));
}
WriteLine("--------------------------------------------------------------");
/*r.Close();
connection.Close();*/
await r.CloseAsync();
WriteLine($"Output count: {p2.Value}");
WriteLine($"Return Value: {p3.Value}");
await connection.CloseAsync();
