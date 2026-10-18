import { formatPercent } from "@/lib/format";

interface Props {
  value: number | null; // 0..1
  threshold: number; // 0..1, drawn as a tick so you can see why a room is flagged
}

/** Single-hue progress track + the number as text (colour never carries meaning alone) */
export function UtilizationMeter({ value, threshold }: Props) {
  if (value === null) return "";

  return (
    <div className="flex items-center gap-3">
      <div
        className="bg-muted relative h-2 w-28 rounded-full"
        role="meter"
        aria-valuemin={0}
        aria-valuemax={100}
        aria-valuenow={Math.round(value * 100)}
        aria-label="Utilization"
      >
        <div
          className="bg-primary/80 h-full rounded-full"
          style={{ width: `${Math.min(value, 1) * 100}%` }}
        />
        <div
          className="bg-foreground/40 absolute -top-1 -bottom-1 w-px"
          style={{ left: `${threshold * 100}%` }}
          title={`Threshold: ${formatPercent(threshold)}`}
        />
      </div>
      <span className="w-10 text-right tabular-nums">
        {formatPercent(value)}
      </span>
    </div>
  );
}
