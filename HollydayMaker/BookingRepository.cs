using MySqlConnector;

namespace HolidayMaker;

public class BookingRepository(MySqlConnection connection)
{
    // CREATE: sparar en bokning och returnerar bokningsnumret 
    public async Task<int> CreateBookingAsync(int customerId, int roomId, DateOnly checkIn, DateOnly checkOut,
        int guests)
    {
        await using var command = new MySqlCommand(
            """
            INSERT INTO bookings (customer_id, room_id, start_date, end_date, number_of_guests)
            SELECT @customerId, @roomId, @checkIn, @checkOut, @guests
            FROM rooms
            WHERE rooms.id = @roomId AND rooms.capacity >= @guests;
            """, connection);
        command.Parameters.AddWithValue("@customerId", customerId);
        command.Parameters.AddWithValue("@roomId", roomId);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);
        command.Parameters.AddWithValue("@guests", guests);
        await command.PrepareAsync();
        int rowsAffected = await command.ExecuteNonQueryAsync();
        if (rowsAffected == 0)
        {
            return 0; // ingen bokning skapad: rummet finns inte eller har inte plats
        }

        return checked((int)command.LastInsertedId); // det nya id:t = bokningsnumret
    }

    // READ: Kontroll innan bokning sparas för att undvika krock.
    
    public async Task<bool> IsRoomAvailableAsync(int roomId, DateOnly checkIn, DateOnly checkOut)
    {
        //Kontroll om den nya bokningen börjar innan den befintliga slutar + om den nya bokningen slutar efter den befintliga börjat.
        //Räknar hur många som uppfyller villkoret. Om 0 => rummet är ledigt. 
        await using var command = new MySqlCommand(
            """
            SELECT COUNT(*) FROM bookings
            WHERE room_id = @roomId
              AND @checkIn < end_date AND @checkOut > start_date; 
            """, connection);
        command.Parameters.AddWithValue("@roomId", roomId);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);
        await command.PrepareAsync();

        object? result = await command.ExecuteScalarAsync();
        long count = Convert.ToInt64(result);
        bool isAvailable = count == 0;
        return isAvailable;
        ; // 0 krockar = ledigt
    }

    //DELETE: Avbokning
    public async Task<int> DeleteBookingAsync(int bookingId)
    {
        await using var command = new MySqlCommand(
            """
            DELETE FROM bookings
            WHERE id = @bookingId;
            """, connection);
        command.Parameters.AddWithValue("@bookingId", bookingId);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected;
    }

    //READ: Visa bokningar
    public async Task<List<BookingInfo>> GetBookingsForCustomerAsync(int customerId)
    {

        await using var command = new MySqlCommand(
            """
            SELECT bookings.id, hotels.name AS hotel_name, hotels.city, rooms.name AS room_name,
                   bookings.start_date, bookings.end_date, bookings.number_of_guests,
                   DATEDIFF(bookings.end_date, bookings.start_date) * rooms.price AS total_price
            FROM bookings JOIN rooms ON bookings.room_id = rooms.id
            JOIN hotels ON rooms.hotel_id = hotels.id
            WHERE bookings.customer_id = @customerId
            ORDER BY bookings.start_date;
            """, connection);
        command.Parameters.AddWithValue("@customerId", customerId);
        await using var reader = await command.ExecuteReaderAsync();
        var bookings = new List<BookingInfo>();

        while (await reader.ReadAsync())
        {
            bookings.Add(new BookingInfo(
                    reader.GetInt32("id"),
                    reader.GetString("hotel_name"),
                    reader.GetString("city"),
                    reader.GetString("room_name"),
                    reader.GetDateOnly("start_date"),
                    reader.GetDateOnly("end_date"),
                    reader.GetInt32("number_of_guests"),
                    reader.GetDecimal("total_price")
                ))
                ;
        }

        return bookings;
    }

    //UPDATE: Antra antal gäster, måste rymmas i rummet
    public async Task<int> UpdateGuestsAsync(int bookingId, int numberOfGuests)
    //Subqueryn hämtar kapacitet för den här bokningens rum. Om antalet är för stort uppdateras 0
    //rader. 
    {
    await using var command = new MySqlCommand(
        """
        UPDATE bookings
        SET number_of_guests = @numberOfGuests
        WHERE id = @bookingId
        AND @numberOfGuests <= (SELECT capacity FROM rooms WHERE rooms.id = bookings.room_id);
        """, connection);
    command.Parameters.AddWithValue("@bookingId", bookingId);
    command.Parameters.AddWithValue("@numberOfGuests", numberOfGuests);

    int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected;
}

}