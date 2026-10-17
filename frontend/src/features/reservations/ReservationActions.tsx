import { toast } from "sonner";
import { getErrorMessages } from "@/api/client";
import type { Reservation } from "@/api/types";
import { ConfirmButton } from "@/components/ConfirmButton";
import { useReservationAction } from "./api";

const showError = (error: unknown) =>
  toast.error(getErrorMessages(error).join(" "));

export function ReservationActions({
  reservation,
}: {
  reservation: Reservation;
}) {
  const cancel = useReservationAction("cancel");
  const endEarly = useReservationAction("end-early");
  const busy = cancel.isPending || endEarly.isPending;

  if (!reservation.canCancel && !reservation.canEndEarly) return null;

  return (
    <div className="flex justify-end gap-2">
      {reservation.canEndEarly && (
        <ConfirmButton
          title="End this meeting now?"
          description={`"${reservation.title}" will be marked as completed and ${reservation.roomName} becomes free immediately.`}
          confirmLabel="End meeting"
          disabled={busy}
          onConfirm={() =>
            endEarly.mutate(reservation.id, {
              onSuccess: () =>
                toast.success(`"${reservation.title}" ended early.`),
              onError: showError,
            })
          }
        >
          End early
        </ConfirmButton>
      )}

      {reservation.canCancel && (
        <ConfirmButton
          title="Cancel this reservation?"
          description={`"${reservation.title}" in ${reservation.roomName} will be cancelled and the slot released.`}
          confirmLabel="Cancel reservation"
          destructive
          disabled={busy}
          onConfirm={() =>
            cancel.mutate(reservation.id, {
              onSuccess: () =>
                toast.success(`"${reservation.title}" was cancelled.`),
              onError: showError,
            })
          }
        >
          Cancel
        </ConfirmButton>
      )}
    </div>
  );
}
