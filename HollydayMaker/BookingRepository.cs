using MySqlConnector;

namespace HolidayMaker;

public class BookingRepository(MySqlConnection connection)
{
    // CREATE: sparar en bokning och returnerar bokningsnumret (punkt 3)
    public async Task<int> CreateBookingAsync(int customerId, int roomId, DateOnly checkIn, DateOnly checkOut, int guests)
    {
        await using var command = new MySqlCommand(
            """
            INSERT INTO bookings (customer_id, room_id, start_date, end_date, number_of_guests)
            VALUES (@customerId, @roomId, @checkIn, @checkOut, @guests);
            """, connection);
        command.Parameters.AddWithValue("@customerId", customerId);
        command.Parameters.AddWithValue("@roomId", roomId);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);
        command.Parameters.AddWithValue("@guests", guests);
        await command.PrepareAsync();
        await command.ExecuteNonQueryAsync();
        return checked((int)command.LastInsertedId); // det nya id:t = bokningsnumret
    }
    
    // Kontroll innan bokning sparas för att undvika krock. 
    public async Task<bool> IsRoomAvailableAsync(int roomId, DateOnly checkIn, DateOnly checkOut)
    {
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
        return isAvailable;; // 0 krockar = ledigt
    }
}