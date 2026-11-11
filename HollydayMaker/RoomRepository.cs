using MySqlConnector;

namespace HolidayMaker;

public class RoomRepository(MySqlConnection connection)
{
    // READ: visar lediga rum på en ort, med plats för sällskapet under hela perioden (punkt 2)
    public async Task<List<AvailableRoom>> SearchAvailableRoomsAsync(
        string city, DateOnly checkIn, DateOnly checkOut, int guests)
    {
        await using var command = new MySqlCommand(
            """
            SELECT rooms.id, hotels.name AS hotel_name, hotels.city,
                   rooms.name AS room_name, rooms.capacity, rooms.price
            FROM hotels
                JOIN rooms ON hotels.id = rooms.hotel_id
            WHERE hotels.city = @city
              AND rooms.capacity >= @guests
              AND rooms.id NOT IN
                  (SELECT room_id FROM bookings
                   WHERE @checkIn < end_date AND @checkOut > start_date)
            ORDER BY rooms.price;
            """, connection);
        command.Parameters.AddWithValue("@city", city);
        command.Parameters.AddWithValue("@guests", guests);
        command.Parameters.AddWithValue("@checkIn", checkIn);
        command.Parameters.AddWithValue("@checkOut", checkOut);
        await command.PrepareAsync();
        await using var reader = await command.ExecuteReaderAsync();

        var rooms = new List<AvailableRoom>();           
        while (await reader.ReadAsync())
        {
            rooms.Add(new AvailableRoom(
                reader.GetInt32("id"),
                reader.GetString("hotel_name"),
                reader.GetString("city"),
                reader.GetString("room_name"),
                reader.GetInt32("capacity"),
                reader.GetDecimal("price")));
        }
        return rooms;         //Lista på tillgängliga rum som rymmer hela sällskapet
    }
}