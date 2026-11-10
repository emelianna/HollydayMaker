namespace HolidayMaker;


public sealed record BookingInfo(int BookingNumber, string HotelName, string City, string RoomName,
    DateOnly StartDate, DateOnly EndDate, int NumberOfGuests, decimal TotalPrice);