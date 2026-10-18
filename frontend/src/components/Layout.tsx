import { CalendarDaysIcon } from "lucide-react";
import { NavLink, Outlet } from "react-router";
import { Toaster } from "@/components/ui/sonner";
import { cn } from "@/lib/utils";

const NAV_ITEMS = [
  { to: "/reservations", label: "Reservations" },
  { to: "/rooms", label: "Rooms" },
  { to: "/users", label: "Users" },
  { to: "/usage", label: "Usage" },
];

export function Layout() {
  return (
    <div className="bg-muted/40 min-h-screen">
      <header className="bg-background border-b">
        <div className="mx-auto flex h-14 max-w-6xl items-center gap-6 px-4">
          <span className="flex items-center gap-2 font-semibold">
            <CalendarDaysIcon className="size-5" />
            RoomReserve
          </span>
          <nav className="flex gap-1">
            {NAV_ITEMS.map((item) => (
              <NavLink
                key={item.to}
                to={item.to}
                className={({ isActive }) =>
                  cn(
                    "rounded-md px-3 py-1.5 text-sm font-medium transition-colors",
                    isActive
                      ? "bg-secondary text-foreground"
                      : "text-muted-foreground hover:text-foreground",
                  )
                }
              >
                {item.label}
              </NavLink>
            ))}
          </nav>
        </div>
      </header>

      <main className="mx-auto max-w-6xl px-4 py-8">
        <Outlet />
      </main>

      <Toaster
        richColors
        position="top-right"
      />
    </div>
  );
}
