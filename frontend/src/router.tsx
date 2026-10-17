import { createBrowserRouter, Navigate } from "react-router";
import { Layout } from "@/components/Layout";
import { RoomsPage } from "@/features/rooms/RoomsPage";

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
    ],
  },
]);
