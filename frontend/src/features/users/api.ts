import { useQuery } from "@tanstack/react-query";
import { apiFetch } from "@/api/client";
import type { User } from "@/api/types";

export function useUsers() {
  return useQuery({
    queryKey: ["users"],
    queryFn: () => apiFetch<User[]>("/users"),
  });
}

export const fullName = (u: Pick<User, "firstName" | "lastName">) =>
  `${u.firstName} ${u.lastName}`;
