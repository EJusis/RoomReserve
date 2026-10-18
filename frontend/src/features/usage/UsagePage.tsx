import { TriangleAlertIcon } from "lucide-react";
import { useState } from "react";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { ErrorAlert } from "@/components/ErrorAlert";
import { FilterField } from "@/components/FilterField";
import { PageHeader } from "@/components/PageHeader";
import { TableState } from "@/components/TableState";
import { formatHours, formatPercent } from "@/lib/format";
import { cn } from "@/lib/utils";
import { lastDays, useRoomUsage, type UsagePeriod } from "./api";
import { StatTile } from "./StatTile";
import { UtilizationMeter } from "./UtilizationMeter";

const PRESETS = [7, 30, 90];

export function UsagePage() {
  const [period, setPeriod] = useState<UsagePeriod>(() => lastDays(30));
  const report = useRoomUsage(period);
  const data = report.data;
  const underused = data?.rooms.filter((r) => r.isUnderused) ?? [];

  return (
    <>
      <PageHeader
        title="Room usage"
        description="Booked time vs. time actually used. Only time that has already passed is counted."
      />

      <Card className="mb-4 py-4">
        <CardContent className="flex flex-wrap items-end gap-4 px-4">
          <FilterField
            label="From"
            htmlFor="usage-from"
          >
            <Input
              id="usage-from"
              type="date"
              className="w-40"
              value={period.from}
              max={period.to}
              onChange={(e) =>
                e.target.value && setPeriod({ ...period, from: e.target.value })
              }
            />
          </FilterField>
          <FilterField
            label="To"
            htmlFor="usage-to"
          >
            <Input
              id="usage-to"
              type="date"
              className="w-40"
              value={period.to}
              min={period.from}
              onChange={(e) =>
                e.target.value && setPeriod({ ...period, to: e.target.value })
              }
            />
          </FilterField>
          <div className="flex gap-2">
            {PRESETS.map((days) => (
              <Button
                key={days}
                variant="outline"
                size="sm"
                onClick={() => setPeriod(lastDays(days))}
              >
                Last {days} days
              </Button>
            ))}
          </div>
        </CardContent>
      </Card>

      <ErrorAlert
        error={report.error}
        title="Could not load the usage report"
      />

      {data && (
        <div className="mb-4 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <StatTile
            label="Booked"
            value={formatHours(data.totalBookedHours)}
            hint="Excluding cancelled meetings"
          />
          <StatTile
            label="Actually used"
            value={formatHours(data.totalUsedHours)}
            hint="Meetings ended early count until they ended"
          />
          <StatTile
            label="Overall utilization"
            value={
              data.overallUtilization === null
                ? "-"
                : formatPercent(data.overallUtilization)
            }
            hint="Used ÷ booked"
          />
          <StatTile
            label="Rarely used rooms"
            value={String(underused.length)}
            hint={`Utilization below ${formatPercent(data.lowUtilizationThreshold)}`}
          />
        </div>
      )}

      {underused.length > 0 && (
        <Alert className="mb-4 border-amber-300 bg-amber-50 text-amber-900">
          <TriangleAlertIcon />
          <AlertTitle>Often booked, rarely used</AlertTitle>
          <AlertDescription className="text-amber-900/80">
            <ul className="list-disc pl-4">
              {underused.map((r) => (
                <li key={r.roomId}>
                  <strong>{r.roomName}</strong> was booked for{" "}
                  {formatHours(r.bookedHours)} but used for only{" "}
                  {formatHours(r.usedHours)} (
                  {formatPercent(r.utilization ?? 0)}). Ended early:{" "}
                  {r.endedEarlyCount} of {r.meetingCount} meetings. Cancelled:{" "}
                  {r.cancelledCount}.
                </li>
              ))}
            </ul>
          </AlertDescription>
        </Alert>
      )}

      <Card
        className={cn(
          "py-0 transition-opacity",
          report.isPlaceholderData && "opacity-60",
        )}
      >
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="pl-4">Room</TableHead>
              <TableHead className="text-right">Meetings</TableHead>
              <TableHead className="text-right">Booked</TableHead>
              <TableHead className="text-right">Used</TableHead>
              <TableHead>Utilization</TableHead>
              <TableHead className="text-right">Ended early</TableHead>
              <TableHead className="text-right">Cancelled</TableHead>
              <TableHead className="pr-4">Insight</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {data?.rooms.map((r) => (
              <TableRow
                key={r.roomId}
                className={cn(!r.isActive && "text-muted-foreground")}
              >
                <TableCell className="pl-4">
                  <div className="font-medium">{r.roomName}</div>
                  <div className="text-muted-foreground text-xs">
                    Floor {r.floor} · {r.capacity} seats
                    {!r.isActive && " · inactive"}
                  </div>
                </TableCell>
                <TableCell className="text-right tabular-nums">
                  {r.meetingCount}
                </TableCell>
                <TableCell className="text-right tabular-nums">
                  {formatHours(r.bookedHours)}
                </TableCell>
                <TableCell className="text-right tabular-nums">
                  {formatHours(r.usedHours)}
                </TableCell>
                <TableCell>
                  <UtilizationMeter
                    value={r.utilization}
                    threshold={data.lowUtilizationThreshold}
                  />
                </TableCell>
                <TableCell className="text-right tabular-nums">
                  {r.endedEarlyCount}
                </TableCell>
                <TableCell className="text-right tabular-nums">
                  {r.cancelledCount}
                </TableCell>
                <TableCell className="pr-4">
                  {r.isUnderused ? (
                    <Badge
                      variant="outline"
                      className="gap-1 border-amber-300 bg-amber-50 text-amber-800"
                    >
                      <TriangleAlertIcon />
                      Rarely used
                    </Badge>
                  ) : null}
                </TableCell>
              </TableRow>
            ))}
            <TableState
              colSpan={8}
              loading={report.isPending}
              empty={data?.rooms.length === 0}
              emptyText="No rooms."
            />
          </TableBody>
        </Table>
      </Card>
    </>
  );
}
