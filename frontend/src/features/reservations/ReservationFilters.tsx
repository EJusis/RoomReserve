import type { ReservationStatus, Room, User } from "@/api/types";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { FilterField } from "@/components/FilterField";
import { ALL, STATUS_OPTIONS } from "@/lib/labels";
import { fullName } from "@/features/users/api";
import { NO_FILTER, type ReservationFilter } from "./api";

interface Props {
  value: ReservationFilter;
  rooms: Room[];
  users: User[];
  onChange: (filter: ReservationFilter) => void;
}

const idOrUndefined = (v: string) => (v === ALL ? undefined : Number(v));

export function ReservationFilters({ value, rooms, users, onChange }: Props) {
  return (
    <Card className="mb-4 py-4">
      <CardContent className="flex flex-wrap items-end gap-4 px-4">
        <FilterField label="Room">
          <Select
            value={value.roomId?.toString() ?? ALL}
            onValueChange={(v) =>
              onChange({ ...value, roomId: idOrUndefined(v) })
            }
          >
            <SelectTrigger
              className="w-40"
              aria-label="Room"
            >
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={ALL}>All rooms</SelectItem>
              {rooms.map((room) => (
                <SelectItem
                  key={room.id}
                  value={room.id.toString()}
                >
                  {room.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FilterField>

        <FilterField label="Organizer">
          <Select
            value={value.organizerId?.toString() ?? ALL}
            onValueChange={(v) =>
              onChange({ ...value, organizerId: idOrUndefined(v) })
            }
          >
            <SelectTrigger
              className="w-48"
              aria-label="Organizer"
            >
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={ALL}>All organizers</SelectItem>
              {users.map((user) => (
                <SelectItem
                  key={user.id}
                  value={user.id.toString()}
                >
                  {fullName(user)}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FilterField>

        <FilterField label="Status">
          <Select
            value={value.status ?? ALL}
            onValueChange={(v) =>
              onChange({
                ...value,
                status: v === ALL ? undefined : (v as ReservationStatus),
              })
            }
          >
            <SelectTrigger
              className="w-36"
              aria-label="Status"
            >
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={ALL}>All statuses</SelectItem>
              {STATUS_OPTIONS.map((status) => (
                <SelectItem
                  key={status}
                  value={status}
                >
                  {status}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FilterField>

        <FilterField
          label="From"
          htmlFor="from"
        >
          <Input
            id="from"
            type="date"
            className="w-40"
            value={value.from ?? ""}
            onChange={(e) =>
              onChange({ ...value, from: e.target.value || undefined })
            }
          />
        </FilterField>

        <FilterField
          label="To"
          htmlFor="to"
        >
          <Input
            id="to"
            type="date"
            className="w-40"
            min={value.from}
            value={value.to ?? ""}
            onChange={(e) =>
              onChange({ ...value, to: e.target.value || undefined })
            }
          />
        </FilterField>

        <Button
          variant="ghost"
          className="ml-auto"
          onClick={() => onChange(NO_FILTER)}
        >
          Clear filters
        </Button>
      </CardContent>
    </Card>
  );
}
