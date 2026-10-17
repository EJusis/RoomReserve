import { PlusIcon } from "lucide-react";
import { useState, type FormEvent } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { ErrorAlert } from "@/components/ErrorAlert";
import { localInputToIso, nextFullHour, toLocalInputValue } from "@/lib/dates";
import { useRooms } from "@/features/rooms/api";
import { fullName, useUsers } from "@/features/users/api";
import { useCreateReservation } from "./api";

interface FormState {
  title: string;
  roomId: string;
  organizerId: string;
  start: string; // "YYYY-MM-DDTHH:mm", local time
  end: string;
  attendeeCount: string;
}

function initialForm(): FormState {
  const start = nextFullHour();
  const end = new Date(start.getTime() + 60 * 60 * 1000);
  return {
    title: "",
    roomId: "",
    organizerId: "",
    start: toLocalInputValue(start),
    end: toLocalInputValue(end),
    attendeeCount: "2",
  };
}

export function CreateReservationDialog() {
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<FormState>(initialForm);
  const rooms = useRooms();
  const users = useUsers();
  const create = useCreateReservation();

  const set = <K extends keyof FormState>(key: K, value: FormState[K]) =>
    setForm((f) => ({ ...f, [key]: value }));

  function handleOpenChange(next: boolean) {
    setOpen(next);
    if (next) {
      setForm(initialForm()); // fresh form every time
      create.reset(); // forget the previous attempt's errors
    }
  }

  // Only checks that the form is filled in. Whether the booking is ALLOWED is decided by the back end.
  const isComplete =
    form.title.trim() !== "" &&
    form.roomId !== "" &&
    form.organizerId !== "" &&
    form.start !== "" &&
    form.end !== "" &&
    form.attendeeCount !== "";

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    create.mutate(
      {
        title: form.title.trim(),
        roomId: Number(form.roomId),
        organizerId: Number(form.organizerId),
        start: localInputToIso(form.start),
        end: localInputToIso(form.end),
        attendeeCount: Number(form.attendeeCount),
      },
      {
        onSuccess: (reservation) => {
          toast.success(
            `"${reservation.title}" booked in ${reservation.roomName}.`,
          );
          setOpen(false);
        },
      },
    );
  }

  return (
    <Dialog
      open={open}
      onOpenChange={handleOpenChange}
    >
      <DialogTrigger asChild>
        <Button>
          <PlusIcon />
          New reservation
        </Button>
      </DialogTrigger>

      <DialogContent className="sm:max-w-xl">
        <form
          onSubmit={handleSubmit}
          className="grid gap-4"
        >
          <DialogHeader>
            <DialogTitle>New reservation</DialogTitle>
            <DialogDescription>
              The booking rules are checked by the server when you save.
            </DialogDescription>
          </DialogHeader>

          <ErrorAlert
            error={create.error}
            title="The reservation could not be created"
          />

          <div className="grid gap-2">
            <Label htmlFor="title">Meeting title</Label>
            <Input
              id="title"
              value={form.title}
              maxLength={200}
              onChange={(e) => set("title", e.target.value)}
              autoFocus
            />
          </div>

          <div className="grid gap-4 sm:grid-cols-2">
            <div className="grid gap-2">
              <Label>Room</Label>
              <Select
                value={form.roomId}
                onValueChange={(v) => set("roomId", v)}
              >
                <SelectTrigger
                  className="w-full"
                  aria-label="Room"
                >
                  <SelectValue placeholder="Choose a room" />
                </SelectTrigger>
                <SelectContent>
                  {rooms.data
                    ?.filter((room) => room.isActive)
                    .map((room) => (
                      <SelectItem
                        key={room.id}
                        value={room.id.toString()}
                      >
                        {room.name} · {room.capacity} seats · floor {room.floor}
                      </SelectItem>
                    ))}
                </SelectContent>
              </Select>
            </div>

            <div className="grid gap-2">
              <Label>Organizer</Label>
              <Select
                value={form.organizerId}
                onValueChange={(v) => set("organizerId", v)}
              >
                <SelectTrigger
                  className="w-full"
                  aria-label="Organizer"
                >
                  <SelectValue placeholder="Choose a person" />
                </SelectTrigger>
                <SelectContent>
                  {users.data?.map((user) => (
                    <SelectItem
                      key={user.id}
                      value={user.id.toString()}
                    >
                      {fullName(user)}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="grid gap-2">
              <Label htmlFor="start">Start</Label>
              <Input
                id="start"
                type="datetime-local"
                value={form.start}
                onChange={(e) => set("start", e.target.value)}
              />
            </div>

            <div className="grid gap-2">
              <Label htmlFor="end">End</Label>
              <Input
                id="end"
                type="datetime-local"
                value={form.end}
                onChange={(e) => set("end", e.target.value)}
              />
            </div>

            <div className="grid gap-2">
              <Label htmlFor="attendees">Attendees</Label>
              <Input
                id="attendees"
                type="number"
                min={1}
                value={form.attendeeCount}
                onChange={(e) => set("attendeeCount", e.target.value)}
              />
            </div>
          </div>

          <DialogFooter>
            <DialogClose asChild>
              <Button
                type="button"
                variant="outline"
              >
                Close
              </Button>
            </DialogClose>
            <Button
              type="submit"
              disabled={!isComplete || create.isPending}
            >
              {create.isPending ? "Saving…" : "Create reservation"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
