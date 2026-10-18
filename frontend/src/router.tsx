import { createBrowserRouter, Navigate } from "react-router";
import { Layout } from "@/components/Layout";
import { RoomsPage } from "@/features/rooms/RoomsPage";
import { UsersPage } from "@/features/users/UsersPage";
import { ReservationsPage } from "@/features/reservations/ReservationsPage";
import { UsagePage } from "@/features/usage/UsagePage";

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
      { path: "usage", element: <UsagePage /> },
    ],
  },
]);
