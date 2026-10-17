import type { ReservationStatus } from "@/api/types";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";

const STATUS_STYLES: Record<ReservationStatus, string> = {
  Booked:
    "border-blue-200 bg-blue-50 text-blue-700 dark:border-blue-900 dark:bg-blue-950 dark:text-blue-300",
  Completed:
    "border-emerald-200 bg-emerald-50 text-emerald-700 dark:border-emerald-900 dark:bg-emerald-950 dark:text-emerald-300",
  Cancelled:
    "border-rose-200 bg-rose-50 text-rose-700 dark:border-rose-900 dark:bg-rose-950 dark:text-rose-300",
};

export function StatusBadge({ status }: { status: ReservationStatus }) {
  return (
    <Badge
      variant="outline"
      className={cn("gap-1.5", STATUS_STYLES[status])}
    >
      <span
        className="size-1.5 rounded-full bg-current"
        aria-hidden
      />
      {status}
    </Badge>
  );
}
