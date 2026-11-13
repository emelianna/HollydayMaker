using MySqlConnector;

namespace HolidayMaker;

public class CustomerRepository(MySqlConnection connection)
{
    // READ: alla kunder i en lista, så att operatören kan välja (punkt 1)
    public async Task<List<Customer>> GetCustomersAsync()
    {
        await using var command = new MySqlCommand(
            """
            SELECT id, first_name, last_name, phone_number, email
            FROM customers
            ORDER BY last_name;
            """, connection);
        await using var reader = await command.ExecuteReaderAsync();

        var customers = new List<Customer>();

        while (await reader.ReadAsync()) // så länge det finns en rad till, läs den
        {
            customers.Add(new Customer(
                reader.GetInt32("id"),
                reader.GetString("first_name"),
                reader.GetString("last_name"),
                reader.GetString("phone_number"),
                reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email")));
        }

        return customers;
    }

    // CREATE: id genereras av databasen.
    public async Task<int> CreateCustomerAsync(string firstName, string lastName, string phoneNumber, string? email)
    {
        await using var command = new MySqlCommand(
            """
            INSERT INTO customers (first_name, last_name, email, phone_number)
            VALUES (@firstName, @lastName, @email, @phoneNumber);
            """, connection);
        command.Parameters.AddWithValue("@firstName", firstName);
        command.Parameters.AddWithValue("@lastName", lastName);
        command.Parameters.AddWithValue("@email",
            email ?? (object)DBNull.Value); //använd email, eller DBNull om den saknas
        command.Parameters.AddWithValue("@phoneNumber", phoneNumber);
        await command.PrepareAsync();
        await command.ExecuteNonQueryAsync();
        return checked((int)command.LastInsertedId); //frågar "vilket id fick raden du nyss skapade?
    }
}