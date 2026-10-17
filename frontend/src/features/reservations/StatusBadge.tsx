import type { ReservationStatus } from "@/api/types";
import { Badge } from "@/components/ui/badge";

const VARIANTS = {
  Booked: "default",
  Completed: "secondary",
  Cancelled: "outline",
} as const satisfies Record<
  ReservationStatus,
  "default" | "secondary" | "outline"
>;

export function StatusBadge({ status }: { status: ReservationStatus }) {
  return (
    <Badge
      variant={VARIANTS[status]}
      className={status === "Cancelled" ? "text-muted-foreground" : undefined}
    >
      {status}
    </Badge>
  );
}
