using MySqlConnector;
using HolidayMaker;

string connectionString = "Server=localhost;Port=3307;Database=holiday_maker;User ID=root;Password=kurslosenord;UseAffectedRows=false;";

await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();
Console.WriteLine("Ansluten till databasen!");

var customerRepository = new CustomerRepository(connection);

List<Customer> customers = await customerRepository.GetCustomersAsync();
foreach (Customer customer in customers)
{
    Console.WriteLine(customer);
}

