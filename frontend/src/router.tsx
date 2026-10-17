import { createBrowserRouter, Navigate } from "react-router";
import { Layout } from "@/components/Layout";
import { RoomsPage } from "@/features/rooms/RoomsPage";
import { UsersPage } from "@/features/users/UsersPage";
import { ReservationsPage } from "@/features/reservations/ReservationsPage";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <Layout />,
    children: [
      {
        index: true,
        element: (
          <Navigate
            to="/reservations"
            replace
          />
        ),
      },
      { path: "rooms", element: <RoomsPage /> },
      { path: "users", element: <UsersPage /> },
      { path: "reservations", element: <ReservationsPage /> },
    ],
  },
]);
