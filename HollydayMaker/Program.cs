using MySqlConnector;
using HolidayMaker;

string connectionString = "Server=localhost;Port=3307;Database=holiday_maker;User ID=root;Password=kurslosenord;UseAffectedRows=false;";

await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();
Console.WriteLine("Ansluten till databasen!");

var customerRepository = new CustomerRepository(connection);

/*List<Customer> customers = await customerRepository.GetCustomersAsync();
foreach (Customer customer in customers)
{
    Console.WriteLine(customer);
}*/

var roomRepository = new RoomRepository(connection);

/*List<AvailableRoom> rooms = await roomRepository.SearchAvailableRoomsAsync(
    "Malmö", new DateOnly(2026, 12, 10), new DateOnly(2026, 12, 13), 2);

Console.WriteLine($"Lediga rum: {rooms.Count}");
foreach (AvailableRoom room in rooms)
{
    Console.WriteLine(room);
}*/

var bookingRepository = new BookingRepository(connection); 

/*int customerId = 6;
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
}
*/
    
    // Test: avbokning frigör rummet
//int bookingToDelete = 26;
//int deleted = await bookingRepository.DeleteBookingAsync(bookingToDelete);
//Console.WriteLine($"Borttagna bokningar: {deleted}");

//List<AvailableRoom> roomsAfter = await roomRepository.SearchAvailableRoomsAsync(
  //  "Malmö", new DateOnly(2026, 12, 10), new DateOnly(2026, 12, 13), 2);
//Console.WriteLine($"Lediga rum efter avbokning: {roomsAfter.Count}");


// Test: visa kundens bokningar (punkt 4)
//int customerToShow = 6;
//List<BookingInfo> customerBookings = await bookingRepository.GetBookingsForCustomerAsync(customerToShow);
//Console.WriteLine($"Bokningar för kund {customerToShow}: {customerBookings.Count}");
//foreach (BookingInfo booking in customerBookings)
//{
//    Console.WriteLine(booking);
//}

//Test: ändra antal personer (punkt 5)
  
/*int bookingToChange = 1;

  int tooManyGuests = 3;
  int result1 = await bookingRepository.UpdateGuestsAsync(bookingToChange, tooManyGuests);
  Console.WriteLine($"Ändra till {tooManyGuests} gäster: {result1} rad(er) ändrade");

  int fits = 2;
  int result2 = await bookingRepository.UpdateGuestsAsync(bookingToChange, fits);
  Console.WriteLine($"Ändra till {fits} gäster: {result2} rad(er) ändrade");*/
  
  // ===== MENY =====
  bool running = true;

  while (running)
  {
      Console.WriteLine();
      Console.WriteLine("***** HOLIDAY MAKER *****");
      Console.WriteLine("1. Välj kund");
      Console.WriteLine("2. Sök lediga rum");
      Console.WriteLine("3. Boka");
      Console.WriteLine("4. Visa bokningar");
      Console.WriteLine("5. Ändra antal personer");
      Console.WriteLine("6. Avboka");
      Console.WriteLine("0. Avsluta");
      Console.Write("Välj: ");

      string? choice = Console.ReadLine();

      switch (choice)
      {
          case "1":
              Console.WriteLine("Du valde 1");
              break;
          case "2":
              Console.WriteLine("Du valde 2");
              break;
          case "3":
              Console.WriteLine("Du valde 3");
              break;
          case "4":
              Console.WriteLine("Du valde 4");
              break;
          case "5":
              Console.WriteLine("Du valde 5");
              break;
          case "6":
              Console.WriteLine("Du valde 6");
              break;
          
          case "0":
              running = false;
              Console.WriteLine("Hej då!");
              break;
          default:
              Console.WriteLine("Ogiltigt val, försök igen.");
              break;
      }
  }