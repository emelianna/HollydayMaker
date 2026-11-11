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
    "Malmö", new DateOnly(2026, 12, 10), new DateOnly(2026, 12, 13), 2);

Console.WriteLine($"Lediga rum: {rooms.Count}");
foreach (AvailableRoom room in rooms)
{
    Console.WriteLine(room);
}

var bookingRepository = new BookingRepository(connection); //Test för att se så dubbelbokningar ej är möjliga

int customerId = 6;
int roomId = 8;
DateOnly checkIn = new DateOnly(2026, 12, 8);
DateOnly checkOut = new DateOnly(2026, 12, 15);
int guests = 2;

bool isAvailable = await bookingRepository.IsRoomAvailableAsync(roomId, checkIn, checkOut);
if (isAvailable)
{
    int bookingId = await bookingRepository.CreateBookingAsync(customerId, roomId, checkIn, checkOut, guests);
    Console.WriteLine($"Bokning skapad! Bokningsnummer: {bookingId}");
}
else
{
    Console.WriteLine("Rummet är tyvärr redan bokat under den perioden.");
    
    // Test: avbokning frigör rummet
int bookingToDelete = 26;
int deleted = await bookingRepository.DeleteBookingAsync(bookingToDelete);
Console.WriteLine($"Borttagna bokningar: {deleted}");

List<AvailableRoom> roomsAfter = await roomRepository.SearchAvailableRoomsAsync(
    "Malmö", new DateOnly(2026, 12, 10), new DateOnly(2026, 12, 13), 2);
Console.WriteLine($"Lediga rum efter avbokning: {roomsAfter.Count}");
}

