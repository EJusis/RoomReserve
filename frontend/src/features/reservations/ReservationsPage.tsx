import { useState } from "react";
import { Card } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { ErrorAlert } from "@/components/ErrorAlert";
import { PageHeader } from "@/components/PageHeader";
import { TableState } from "@/components/TableState";
import { formatRange, formatTime } from "@/lib/dates";
import { cn } from "@/lib/utils";
import { useRooms } from "@/features/rooms/api";
import { useUsers } from "@/features/users/api";
import { NO_FILTER, useReservations, type ReservationFilter } from "./api";
import { CreateReservationDialog } from "./CreateReservationDialog";
import { ReservationActions } from "./ReservationActions";
import { ReservationFilters } from "./ReservationFilters";
import { StatusBadge } from "./StatusBadge";

export function ReservationsPage() {
  const [filter, setFilter] = useState<ReservationFilter>(NO_FILTER);
  const reservations = useReservations(filter);
  const rooms = useRooms();
  const users = useUsers();

  return (
    <>
      <PageHeader
        title="Reservations"
        description="Book rooms, cancel upcoming meetings, or end a meeting early to free the room."
        action={<CreateReservationDialog />}
      />

      <ReservationFilters
        value={filter}
        rooms={rooms.data ?? []}
        users={users.data ?? []}
        onChange={setFilter}
      />
      <ErrorAlert
        error={reservations.error}
        title="Could not load reservations"
      />

      <Card
        className={cn(
          "py-0 transition-opacity",
          reservations.isPlaceholderData && "opacity-60",
        )}
      >
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="pl-4">Meeting</TableHead>
              <TableHead>Room</TableHead>
              <TableHead>Organizer</TableHead>
              <TableHead>When</TableHead>
              <TableHead className="text-right">Attendees</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="pr-4 text-right">
                <span className="sr-only">Actions</span>
              </TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {reservations.data?.map((r) => (
              <TableRow
                key={r.id}
                className={cn(
                  r.status === "Cancelled" && "text-muted-foreground",
                )}
              >
                <TableCell className="pl-4 font-medium">{r.title}</TableCell>
                <TableCell>{r.roomName}</TableCell>
                <TableCell>{r.organizerName}</TableCell>
                <TableCell>
                  <div
                    className={cn(r.status === "Cancelled" && "line-through")}
                  >
                    {formatRange(r.start, r.end)}
                  </div>
                  {r.endedEarlyAt && (
                    <div className="text-muted-foreground text-xs">
                      Ended early at {formatTime(r.endedEarlyAt)}
                    </div>
                  )}
                </TableCell>
                <TableCell className="text-right tabular-nums">
                  {r.attendeeCount}
                </TableCell>
                <TableCell>
                  <StatusBadge status={r.status} />
                </TableCell>
                <TableCell className="pr-4">
                  <ReservationActions reservation={r} />
                </TableCell>
              </TableRow>
            ))}
            <TableState
              colSpan={7}
              loading={reservations.isPending}
              empty={reservations.data?.length === 0}
              emptyText="No reservations match these filters."
            />
          </TableBody>
        </Table>
      </Card>
    </>
  );
}
