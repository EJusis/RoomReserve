export type Equipment =
  | "Projector"
  | "TvScreen"
  | "VideoConferencing"
  | "Whiteboard";

export type ReservationStatus = "Booked" | "Completed" | "Cancelled";

export interface Room {
  id: number;
  name: string;
  capacity: number;
  floor: number;
  location: string;
  equipment: Equipment[];
  isActive: boolean;
}

export interface User {
  id: number;
  firstName: string;
  lastName: string;
  activeReservationCount: number;
}

export interface Reservation {
  id: number;
  roomId: number;
  roomName: string;
  organizerId: number;
  organizerName: string;
  title: string;
  start: string;
  end: string;
  endedEarlyAt: string | null;
  attendeeCount: number;
  status: ReservationStatus;
  canCancel: boolean;
  canEndEarly: boolean;
}

export interface CreateReservationBody {
  roomId: number;
  organizerId: number;
  title: string;
  start: string;
  end: string;
  attendeeCount: number;
}

export interface ApiErrorItem {
  code: string;
  message: string;
}

export interface ProblemDetails {
  status?: number;
  title?: string;
  detail?: string;
  errors?: ApiErrorItem[];
}
