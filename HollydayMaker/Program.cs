using MySqlConnector;
using HolidayMaker;

string connectionString =
    "Server=localhost;Port=3307;Database=holiday_maker;User ID=root;Password=kurslosenord;UseAffectedRows=false;";

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

int selectedCustomerId = 0; //0 = ingen kund vald än
DateOnly? searchIn = null;   // null = ingen sökning gjord än
DateOnly? searchOut = null;
int searchGuests = 0;

while (running)
{
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("***** HOLIDAY MAKER *****");
    Console.ResetColor();
    Console.WriteLine("--Så här bokar du:--");
    Console.WriteLine("1 Välj/registrera kund => 2 Sök lediga rum => 3 Boka med rum-id från sökningen.");
    Console.WriteLine();
    Console.WriteLine("1. Välj kund / registrera ny kund");
    Console.WriteLine("2. Sök lediga rum");
    Console.WriteLine("3. Boka");
    Console.WriteLine();
    Console.WriteLine("--Ärenden gällande befintliga bokningar:--");
    Console.WriteLine("4. Visa bokningar");
    Console.WriteLine("5. Ändra antal personer");
    Console.WriteLine("6. Avboka");
    Console.WriteLine();
    Console.WriteLine("0. Avsluta");
    Console.Write("Välj: ");

    string? choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
                List<Customer> allCustomers = await customerRepository.GetCustomersAsync();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Registrerade kunder:");
                Console.ResetColor();
                foreach (Customer customer in allCustomers)
                {
                    string email = customer.Email ?? "ingen e-post";
                    Console.WriteLine($"Kundnr {customer.Id}: {customer.FirstName} {customer.LastName}, tel {customer.PhoneNumber}, {email}");
                }
                Console.WriteLine();

                Console.Write("Ange kundnummer, n för ny kund (0 = tillbaka till menyn): ");
                string? customerChoiceInput = Console.ReadLine();
                if (customerChoiceInput == "0")
                {
                    break;
                }

                if (customerChoiceInput == "n")
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

                    selectedCustomerId =
                        await customerRepository.CreateCustomerAsync(firstName, lastName, phone, email);
                    Console.WriteLine($"Ny kund skapad med kundnummer {selectedCustomerId}.");
                }
                else
                {
                    selectedCustomerId = int.Parse(customerChoiceInput!);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Kund {selectedCustomerId} är vald. Fortsätt med val 2 om du vill se lediga rum");
                    Console.ResetColor();
                }

                break;

            case "2":
                Console.Write("Vilken ort? (0 = tillbaka till menyn): ");
                string? cityInput = Console.ReadLine();
                if (cityInput == "0")
                {
                    break;
                }

                string city = cityInput!;

                Console.Write("Ange datum för incheckning (ÅÅÅÅ-MM-DD): ");
                string? inDateInput = Console.ReadLine();
                DateOnly inDate = DateOnly.Parse(inDateInput!);

                Console.Write("Ange datum för utcheckning (ÅÅÅÅ-MM-DD): ");
                string? outDateInput = Console.ReadLine();
                DateOnly outDate = DateOnly.Parse(outDateInput!);

                Console.Write("Antal personer: ");
                string? guestsInput = Console.ReadLine();
                int guests = int.Parse(guestsInput!);
                searchIn = inDate;
                searchOut = outDate;
                searchGuests = guests;

                Console.Write("Är pool ett krav? (j/n): ");
                string? poolInput = Console.ReadLine();
                bool poolRequired = poolInput == "j";

                Console.Write("Är restaurang ett krav? (j/n): ");
                string? restaurantInput = Console.ReadLine();
                bool restaurantRequired = restaurantInput == "j";

                Console.Write("Max avstånd till strand i meter (Enter = spelar ingen roll): ");
                string? beachInput = Console.ReadLine();
                int? maxBeach = null;
                if (beachInput != "")
                {
                    maxBeach = int.Parse(beachInput!);
                }

                Console.Write("Max avstånd till centrum i meter (Enter = spelar ingen roll): ");
                string? centerInput = Console.ReadLine();
                int? maxCenter = null;
                if (centerInput != "")
                {
                    maxCenter = int.Parse(centerInput!);
                }

                Console.Write("Sortera efter 1 = pris (lägst först), 2 = omdöme (högst först): ");
                string? sortInput = Console.ReadLine();
                string sortBy = "price";
                if (sortInput == "2")
                {
                    sortBy = "rating";
                }

                List<AvailableRoom> rooms = await roomRepository.SearchAvailableRoomsAsync(city, inDate, outDate,
                    guests,
                    poolRequired,
                    restaurantRequired, maxBeach, maxCenter, sortBy);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Alternativ som passar din sökning:");
                Console.ResetColor();
                
                foreach (AvailableRoom room in rooms)
                {
                    Console.WriteLine(
                        $"Rum-id {room.RoomId}: {room.HotelName}, {room.City} – rum {room.RoomName}, {room.Capacity} pers, {room.Price} kr/natt");
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

                Console.Write("Ange rum-id från sökresultatet: ");
                int bookRoomId = int.Parse(Console.ReadLine()!);

                DateOnly bookIn;
                DateOnly bookOut;
                int bookGuests;

                if (searchIn != null)
                {
                    Console.Write($"Samma datum och antal som sökningen? ({searchIn} – {searchOut}, {searchGuests} pers) Enter = ja, n = nej: ");
                }

                string? sameInput = searchIn != null ? Console.ReadLine() : "n";

                if (sameInput == "")
                {
                    bookIn = searchIn!.Value;
                    bookOut = searchOut!.Value;
                    bookGuests = searchGuests;
                }
                else
                {
                    Console.Write("Incheckning (ÅÅÅÅ-MM-DD): ");
                    bookIn = DateOnly.Parse(Console.ReadLine()!);
                    Console.Write("Utcheckning (ÅÅÅÅ-MM-DD): ");
                    bookOut = DateOnly.Parse(Console.ReadLine()!);
                    Console.Write("Antal personer: ");
                    bookGuests = int.Parse(Console.ReadLine()!);
                }


                bool roomIsFree = await bookingRepository.IsRoomAvailableAsync(bookRoomId, bookIn, bookOut);
                if (roomIsFree)
                {
                    int newBookingId =
                        await bookingRepository.CreateBookingAsync(selectedCustomerId, bookRoomId, bookIn, bookOut,
                            bookGuests);
                    if (newBookingId == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(
                            "Bokningen gick inte: rummet har inte plats för så många, eller så finns inte rum-id:t.");
                        Console.ResetColor();
                    }
                    else
                    {
                        decimal totalPrice = await bookingRepository.GetTotalPriceAsync(newBookingId);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(
                            $"Bokning skapad! Bokningsnummer: {newBookingId}, totalpris: {totalPrice} kr");
                        Console.ResetColor();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Rummet är tyvärr redan bokat under den perioden.");
                    Console.ResetColor();
                }

                break;


            case "4":
                Console.Write("Ange kundnummer (0 = tillbaka till menyn): ");
                string? customerIdInput = Console.ReadLine();
                if (customerIdInput == "0")
                {
                    break;
                }

                int customerIdToShow = int.Parse(customerIdInput!);


                List<BookingInfo> customerBookings =
                    await bookingRepository.GetBookingsForCustomerAsync(customerIdToShow);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Bokningar gjorda med det kundnumret: {customerBookings.Count}");
                Console.ResetColor();
                foreach (BookingInfo booking in customerBookings)
                {
                    Console.WriteLine(booking);
                }

                break;

            case "5":
                Console.Write("Ange bokningsnummer (0 = tillbaka till menyn): ");
                string? changeIdInput = Console.ReadLine();
                if (changeIdInput == "0")
                {
                    break;
                }

                int bookingIdToChange = int.Parse(changeIdInput!);

                Console.Write("Nytt antal personer: ");
                string? newGuestsInput = Console.ReadLine();
                int newGuests = int.Parse(newGuestsInput!);

                int updatedRows = await bookingRepository.UpdateGuestsAsync(bookingIdToChange, newGuests);

                if (updatedRows == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Antalet personer är ändrat.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Det gick inte! Rummet har inte plats, eller så finns inte bokningen.");
                    Console.ResetColor();
                }

                break;


            case "6":
                Console.Write("Ange bokningsnummer att avboka (0 = tillbaka till menyn): ");
                string? bookingIdInput = Console.ReadLine();
                if (bookingIdInput == "0")
                {
                    break;
                }

                int bookingIdToDelete = int.Parse(bookingIdInput!);

                int deletedRows = await bookingRepository.DeleteBookingAsync(bookingIdToDelete);

                if (deletedRows == 1)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Bokningen är avbokad.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ingen bokning med det numret hittades.");
                    Console.ResetColor();
                }

                break;

            case "0":
                running = false;
                Console.WriteLine("Hej då!");
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ogiltigt val, försök igen.");
                Console.ResetColor();
                break;
        }
    }
    catch (Exception)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Något gick fel, troligen felaktig inmatning. Försök igen.");
        Console.ResetColor();
    }
}