import { useMemo, useState } from "react";
import { Badge } from "@/components/ui/badge";
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
import { EQUIPMENT_LABELS } from "@/lib/labels";
import { cn } from "@/lib/utils";
import { ALL_ROOMS, useRooms, type RoomFilter } from "./api";
import { RoomFilters } from "./RoomFilters";

export function RoomsPage() {
  const [filter, setFilter] = useState<RoomFilter>(ALL_ROOMS);
  const rooms = useRooms(filter);

  // Unfiltered list, only used to offer every floor in the dropdown (cached, so cheap)
  const allRooms = useRooms(ALL_ROOMS);
  const floors = useMemo(
    () =>
      [...new Set(allRooms.data?.map((r) => r.floor))].sort((a, b) => a - b),
    [allRooms.data],
  );

  return (
    <>
      <PageHeader
        title="Meeting rooms"
        description="All rooms in the office. Inactive rooms cannot be booked."
      />
      <RoomFilters
        value={filter}
        floors={floors}
        onChange={setFilter}
      />
      <ErrorAlert
        error={rooms.error}
        title="Could not load rooms"
      />

      <Card
        className={cn(
          "py-0 transition-opacity",
          rooms.isPlaceholderData && "opacity-60",
        )}
      >
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="pl-4">ID</TableHead>
              <TableHead>Name</TableHead>
              <TableHead className="text-right">Capacity</TableHead>
              <TableHead>Floor / location</TableHead>
              <TableHead>Equipment</TableHead>
              <TableHead>Status</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {rooms.data?.map((room) => (
              <TableRow
                key={room.id}
                className={cn(!room.isActive && "text-muted-foreground")}
              >
                <TableCell className="pl-4">{room.id}</TableCell>
                <TableCell className="font-medium">{room.name}</TableCell>
                <TableCell className="text-right tabular-nums">
                  {room.capacity}
                </TableCell>
                <TableCell>
                  Floor {room.floor} · {room.location}
                </TableCell>
                <TableCell>
                  <div className="flex flex-wrap gap-1">
                    {room.equipment.length === 0 && (
                      <span className="text-muted-foreground">None</span>
                    )}
                    {room.equipment.map((item) => (
                      <Badge
                        key={item}
                        variant="outline"
                      >
                        {EQUIPMENT_LABELS[item]}
                      </Badge>
                    ))}
                  </div>
                </TableCell>
                <TableCell>
                  {room.isActive ? (
                    <Badge className="bg-emerald-600 text-white">Active</Badge>
                  ) : (
                    <Badge variant="secondary">Inactive</Badge>
                  )}
                </TableCell>
              </TableRow>
            ))}
            <TableState
              colSpan={6}
              loading={rooms.isPending}
              empty={rooms.data?.length === 0}
              emptyText="No rooms match these filters."
            />
          </TableBody>
        </Table>
      </Card>
    </>
  );
}
