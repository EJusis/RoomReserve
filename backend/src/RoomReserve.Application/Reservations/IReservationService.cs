using RoomReserve.Application.Common;

namespace RoomReserve.Application.Reservations;

public interface IReservationService
{
    Task<IReadOnlyList<ReservationDto>> GetReservationsAsync(ReservationFilter filter, CancellationToken ct = default);
    Task<Result<ReservationDto>> CreateAsync(CreateReservationRequest request, CancellationToken ct = default);
    Task<Result<ReservationDto>> CancelAsync(int id, CancellationToken ct = default);
    Task<Result<ReservationDto>> EndEarlyAsync(int id, CancellationToken ct = default);
}