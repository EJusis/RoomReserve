import type { Equipment, ReservationStatus } from "@/api/types";

export const EQUIPMENT_OPTIONS: Equipment[] = [
  "Projector",
  "TvScreen",
  "VideoConferencing",
  "Whiteboard",
];

export const EQUIPMENT_LABELS: Record<Equipment, string> = {
  Projector: "Projector",
  TvScreen: "TV screen",
  VideoConferencing: "Video conferencing",
  Whiteboard: "Whiteboard",
};

export const STATUS_OPTIONS: ReservationStatus[] = [
  "Booked",
  "Completed",
  "Cancelled",
];

/** Radix Select can't use "" as an item value, so "all" stands for "no filter" */
export const ALL = "all";
