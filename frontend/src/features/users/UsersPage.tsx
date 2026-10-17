import { Badge } from "@/components/ui/badge";
import { Card } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { ErrorAlert } from "@/components/ErrorAlert";
import { PageHeader } from "@/components/PageHeader";
import { TableState } from "@/components/TableState";
import { useUsers } from "./api";

export function UsersPage() {
  const users = useUsers();

  return (
    <>
      <PageHeader
        title="Users"
        description="Active reservations are booked meetings that have not ended yet."
      />
      <ErrorAlert
        error={users.error}
        title="Could not load users"
      />

      <Card className="py-0">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="pl-4">ID</TableHead>
              <TableHead>First name</TableHead>
              <TableHead>Last name</TableHead>
              <TableHead className="pr-4 text-right">
                Active reservations
              </TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {users.data?.map((user) => (
              <TableRow key={user.id}>
                <TableCell className="pl-4">{user.id}</TableCell>
                <TableCell>{user.firstName}</TableCell>
                <TableCell>{user.lastName}</TableCell>
                <TableCell className="pr-4 text-right">
                  <Badge
                    variant={
                      user.activeReservationCount > 0 ? "default" : "secondary"
                    }
                    className="tabular-nums"
                  >
                    {user.activeReservationCount}
                  </Badge>
                </TableCell>
              </TableRow>
            ))}
            <TableState
              colSpan={4}
              loading={users.isPending}
              empty={users.data?.length === 0}
              emptyText="No users."
            />
          </TableBody>
        </Table>
      </Card>
    </>
  );
}
