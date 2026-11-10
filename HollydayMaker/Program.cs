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

var roomRepository = new RoomRepository(connection);

List<AvailableRoom> rooms = await roomRepository.SearchAvailableRoomsAsync(
    "Båstad", new DateOnly(2026, 12, 22), new DateOnly(2026, 12, 24), 4);

Console.WriteLine($"Lediga rum: {rooms.Count}");
foreach (AvailableRoom room in rooms)
{
    Console.WriteLine(room);
}