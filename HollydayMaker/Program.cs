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
  
  // MENY 
  bool running = true;

  int selectedCustomerId = 0;   // För att komma ihåg vald kund. 0 = ingen kund vald än

  while (running)
  {
      Console.WriteLine();
      Console.WriteLine("***** HOLIDAY MAKER *****");
      Console.WriteLine("1. Välj kund / registrera ny kund");
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
              List<Customer> allCustomers = await customerRepository.GetCustomersAsync();
              foreach (Customer customer in allCustomers)
              {
                  Console.WriteLine(customer);
              }

              Console.Write("Ange kundnummer (eller 0 för ny kund): ");
              string? customerChoiceInput = Console.ReadLine();
              int customerChoice = int.Parse(customerChoiceInput!);

              if (customerChoice == 0)
              {
                  Console.Write("Förnamn: ");
                  string firstName = Console.ReadLine()!;
                  Console.Write("Efternamn: ");
                  string lastName = Console.ReadLine()!;
                  Console.Write("Telefon: ");
                  string phone = Console.ReadLine()!;
                  Console.Write("E-post (tryck Enter för att hoppa över): ");
                  string? emailInput = Console.ReadLine();
                  string? email = null;
                  if (emailInput != "")
                  {
                      email = emailInput;
                  }

                  selectedCustomerId = await customerRepository.CreateCustomerAsync(firstName, lastName, phone, email);
                  Console.WriteLine($"Ny kund skapad med kundnummer {selectedCustomerId}.");
              }
              else
              {
                  selectedCustomerId = customerChoice;
                  Console.WriteLine($"Kund {selectedCustomerId} är vald.");
              }
              break;
              
          case "2":
              Console.WriteLine("Vilken ort?: ");
              string? cityInput = Console.ReadLine();
              string city = cityInput!;
              
              Console.WriteLine("Ange datum för incheckning (ÅÅÅÅ-MM-DD): "); 
              string? inDateInput = Console.ReadLine();
              DateOnly inDate = DateOnly.Parse(inDateInput!);
              
              Console.Write("Ange datum för utcheckning (ÅÅÅÅ-MM-DD): ");
              string? outDateInput = Console.ReadLine();
              DateOnly outDate = DateOnly.Parse(outDateInput!);

              Console.Write("Antal personer: ");
              string? guestsInput = Console.ReadLine();
              int guests = int.Parse(guestsInput!);
              List<AvailableRoom> rooms = await roomRepository.SearchAvailableRoomsAsync(city, inDate, outDate, guests);

              Console.WriteLine("Alternativ som passar din sökning:");
              foreach (AvailableRoom room in rooms)
              {
                  Console.WriteLine(room);
              }
              break;
          
          case "3":
              if (selectedCustomerId == 0)
              {
                  Console.WriteLine("Om bokningen gäller en ny kund, registrera via val 1 först.");
                  Console.Write("Ange kundnummer (eller 0 för att komma tillbaka till menyn): ");
                  string? bookCustomerInput = Console.ReadLine();
                  selectedCustomerId = int.Parse(bookCustomerInput!);
                  if (selectedCustomerId == 0)
                  {
                      break;
                  }
              }

              Console.Write("Rumsnummer (RoomId från sökningen): ");
              int bookRoomId = int.Parse(Console.ReadLine()!);
              Console.Write("Incheckning (ÅÅÅÅ-MM-DD): ");
              DateOnly bookIn = DateOnly.Parse(Console.ReadLine()!);
              Console.Write("Utcheckning (ÅÅÅÅ-MM-DD): ");
              DateOnly bookOut = DateOnly.Parse(Console.ReadLine()!);
              Console.Write("Antal personer: ");
              int bookGuests = int.Parse(Console.ReadLine()!);

              bool roomIsFree = await bookingRepository.IsRoomAvailableAsync(bookRoomId, bookIn, bookOut);
              if (roomIsFree)
              {
                  int newBookingId = await bookingRepository.CreateBookingAsync(selectedCustomerId, bookRoomId, bookIn, bookOut, bookGuests);
                  Console.WriteLine($"Bokning skapad! Bokningsnummer: {newBookingId}");
              }
              else
              {
                  Console.WriteLine("Rummet är tyvärr redan bokat under den perioden.");
              }
              break;
           
         
          case "4":
              Console.Write("Ange kundnummer: ");
              string? customerIdInput = Console.ReadLine();
              int customerIdToShow = int.Parse(customerIdInput!);

              List<BookingInfo> customerBookings = await bookingRepository.GetBookingsForCustomerAsync(customerIdToShow);

              Console.WriteLine($"Bokningar gjorda med det kundnumret: {customerBookings.Count}");
              foreach (BookingInfo booking in customerBookings)
              {
                  Console.WriteLine(booking);
              }
              break;

          case "5":
              Console.Write("Ange bokningsnummer: ");
              string? changeIdInput = Console.ReadLine();
              int bookingIdToChange = int.Parse(changeIdInput!);

              Console.Write("Nytt antal personer: ");
              string? newGuestsInput = Console.ReadLine();
              int newGuests = int.Parse(newGuestsInput!);

              int updatedRows = await bookingRepository.UpdateGuestsAsync(bookingIdToChange, newGuests);

              if (updatedRows == 1)
              {
                  Console.WriteLine("Antalet personer är ändrat.");
              }
              else
              {
                  Console.WriteLine("Det gick inte! Rummet har inte plats, eller så finns inte bokningen.");
              }
              break;
          
         
          case "6":
              Console.Write("Ange bokningsnummer att avboka: ");
              string? bookingIdInput = Console.ReadLine();
              int bookingIdToDelete = int.Parse(bookingIdInput!);

              int deletedRows = await bookingRepository.DeleteBookingAsync(bookingIdToDelete);

              if (deletedRows == 1)
              {
                  Console.WriteLine("Bokningen är avbokad.");
              }
              else
              {
                  Console.WriteLine("Ingen bokning med det numret hittades.");
              }
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