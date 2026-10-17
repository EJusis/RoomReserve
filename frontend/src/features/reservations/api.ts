import {
  keepPreviousData,
  useMutation,
  useQuery,
  useQueryClient,
  type QueryClient,
} from "@tanstack/react-query";
import { apiFetch, toQueryString } from "@/api/client";
import type {
  CreateReservationBody,
  Reservation,
  ReservationStatus,
} from "@/api/types";
import { startOfDayIso, startOfNextDayIso } from "@/lib/dates";

export interface ReservationFilter {
  roomId?: number;
  organizerId?: number;
  status?: ReservationStatus;
  from?: string; // "YYYY-MM-DD"
  to?: string; // "YYYY-MM-DD", inclusive
}

export const NO_FILTER: ReservationFilter = {};

export function useReservations(filter: ReservationFilter) {
  return useQuery({
    queryKey: ["reservations", filter],
    queryFn: () =>
      apiFetch<Reservation[]>(
        `/reservations${toQueryString({
          roomId: filter.roomId,
          organizerId: filter.organizerId,
          status: filter.status,
          from: filter.from && startOfDayIso(filter.from),
          to: filter.to && startOfNextDayIso(filter.to),
        })}`,
      ),
    placeholderData: keepPreviousData,
  });
}

function refreshAfterChange(queryClient: QueryClient) {
  return Promise.all([
    queryClient.invalidateQueries({ queryKey: ["reservations"] }),
    queryClient.invalidateQueries({ queryKey: ["users"] }),
  ]);
}

export function useCreateReservation() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (body: CreateReservationBody) =>
      apiFetch<Reservation>("/reservations", {
        method: "POST",
        body: JSON.stringify(body),
      }),
    onSuccess: () => refreshAfterChange(queryClient),
  });
}

export type ReservationAction = "cancel" | "end-early";

export function useReservationAction(action: ReservationAction) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) =>
      apiFetch<Reservation>(`/reservations/${id}/${action}`, {
        method: "POST",
      }),
    onSuccess: () => refreshAfterChange(queryClient),
  });
}
