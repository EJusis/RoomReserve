import type { ApiErrorItem, ProblemDetails } from "./types";

export class ApiError extends Error {
  readonly status: number;
  readonly errors: ApiErrorItem[];

  constructor(status: number, problem: ProblemDetails | null) {
    super(
      problem?.detail ??
        problem?.title ??
        `Request failed with status ${status}`,
    );
    this.name = "ApiError";
    this.status = status;
    this.errors = problem?.errors ?? [];
  }
}

export function getErrorMessages(error: unknown): string[] {
  if (error instanceof ApiError && error.errors.length > 0)
    return error.errors.map((e) => e.message);
  if (error instanceof Error) return [error.message];
  return ["Something went wrong."];
}

type QueryValue =
  | string
  | number
  | boolean
  | null
  | undefined
  | (string | number)[];

export function toQueryString(params: Record<string, QueryValue>): string {
  const search = new URLSearchParams();

  for (const [key, value] of Object.entries(params)) {
    if (value === undefined || value === null || value === "") continue;
    if (Array.isArray(value))
      value.forEach((v) => search.append(key, String(v)));
    else search.append(key, String(value));
  }

  const query = search.toString();
  return query ? `?${query}` : "";
}

export async function apiFetch<T>(
  path: string,
  init?: RequestInit,
): Promise<T> {
  const response = await fetch(`/api${path}`, {
    ...init,
    headers: { "Content-Type": "application/json", ...init?.headers },
  });

  if (!response.ok) {
    const problem = (await response
      .json()
      .catch(() => null)) as ProblemDetails | null;
    throw new ApiError(response.status, problem);
  }

  if (response.status === 204) return undefined as T;
  return (await response.json()) as T;
}
