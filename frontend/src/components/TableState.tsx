import { TableCell, TableRow } from "@/components/ui/table";

export function TableState({
  colSpan,
  loading,
  empty,
  emptyText,
}: {
  colSpan: number;
  loading: boolean;
  empty: boolean;
  emptyText: string;
}) {
  if (!loading && !empty) return null;
  return (
    <TableRow className="hover:bg-transparent">
      <TableCell
        colSpan={colSpan}
        className="text-muted-foreground h-24 text-center"
      >
        {loading ? "Loading…" : emptyText}
      </TableCell>
    </TableRow>
  );
}
