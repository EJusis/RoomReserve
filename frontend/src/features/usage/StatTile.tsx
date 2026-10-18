import { Card, CardContent } from "@/components/ui/card";

export function StatTile({
  label,
  value,
  hint,
}: {
  label: string;
  value: string;
  hint?: string;
}) {
  return (
    <Card className="py-4 print:break-inside-avoid">
      <CardContent className="px-4">
        <p className="text-muted-foreground text-sm">{label}</p>
        <p className="mt-1 text-2xl font-semibold tabular-nums">{value}</p>
        {hint && <p className="text-muted-foreground mt-1 text-xs">{hint}</p>}
      </CardContent>
    </Card>
  );
}
