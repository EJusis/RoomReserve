import { formatDate } from "@/lib/dates";
import type { UsagePeriod } from "./api";

/** Only visible on paper: what this document is, and when it was produced */
export function PrintHeader({ period }: { period: UsagePeriod }) {
  const generated = new Intl.DateTimeFormat("en-GB", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date());

  return (
    <div className="mb-6 hidden border-b pb-4 print:block">
      <p className="text-muted-foreground text-sm">RoomReserve</p>
      <h1 className="text-2xl font-semibold">Room usage report</h1>
      <p className="text-muted-foreground mt-1 text-sm">
        {formatDate(period.from)} – {formatDate(period.to)} · Generated{" "}
        {generated}
      </p>
    </div>
  );
}
