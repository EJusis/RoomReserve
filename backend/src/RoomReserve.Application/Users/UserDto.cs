namespace RoomReserve.Application.Users;

public record UserDto(int Id, string FirstName, string LastName, int ActiveReservationCount);