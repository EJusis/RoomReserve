import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { apiFetch, toQueryString } from "@/api/client";
import type { Equipment, Room } from "@/api/types";

export interface RoomFilter {
  minCapacity?: number;
  floor?: number;
  equipment: Equipment[];
}

export const ALL_ROOMS: RoomFilter = { equipment: [] };

export function useRooms(filter: RoomFilter = ALL_ROOMS) {
  return useQuery({
    queryKey: ["rooms", filter],
    queryFn: () =>
      apiFetch<Room[]>(
        `/rooms${toQueryString({
          minCapacity: filter.minCapacity,
          floor: filter.floor,
          equipment: filter.equipment,
        })}`,
      ),
    placeholderData: keepPreviousData,
  });
}
