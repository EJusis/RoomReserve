import { keepPreviousData, useQuery } from "@tanstack/react-query";
import { apiFetch, toQueryString } from "@/api/client";
import type { RoomUsageReport } from "@/api/types";
import { localDate, startOfDayIso, startOfNextDayIso } from "@/lib/dates";

export interface UsagePeriod {
  from: string; // "YYYY-MM-DD", local
  to: string; // "YYYY-MM-DD", local, inclusive
}

export const lastDays = (days: number): UsagePeriod => ({
  from: localDate(-(days - 1)),
  to: localDate(),
});

export function useRoomUsage(period: UsagePeriod) {
  return useQuery({
    queryKey: ["reports", "room-usage", period],
    queryFn: () =>
      apiFetch<RoomUsageReport>(
        `/reports/room-usage${toQueryString({
          from: startOfDayIso(period.from),
          to: startOfNextDayIso(period.to),
        })}`,
      ),
    placeholderData: keepPreviousData,
  });
}
