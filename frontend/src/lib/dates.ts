const dateTimeFormat = new Intl.DateTimeFormat("en-GB", {
  weekday: "short",
  day: "numeric",
  month: "short",
  hour: "2-digit",
  minute: "2-digit",
});
const timeFormat = new Intl.DateTimeFormat("en-GB", {
  hour: "2-digit",
  minute: "2-digit",
});

export const formatDateTime = (iso: string) =>
  dateTimeFormat.format(new Date(iso));
export const formatTime = (iso: string) => timeFormat.format(new Date(iso));

export function formatRange(startIso: string, endIso: string) {
  const sameDay =
    new Date(startIso).toDateString() === new Date(endIso).toDateString();
  return `${formatDateTime(startIso)} – ${sameDay ? formatTime(endIso) : formatDateTime(endIso)}`;
}

export function toLocalInputValue(date: Date) {
  const pad = (n: number) => String(n).padStart(2, "0");
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
}

export const localInputToIso = (value: string) => new Date(value).toISOString();

export const startOfDayIso = (date: string) =>
  new Date(`${date}T00:00`).toISOString();

export function startOfNextDayIso(date: string) {
  const d = new Date(`${date}T00:00`);
  d.setDate(d.getDate() + 1);
  return d.toISOString();
}

export function nextFullHour(from = new Date()) {
  const d = new Date(from);
  d.setHours(d.getHours() + 1, 0, 0, 0);
  return d;
}

export function localDate(offsetDays = 0) {
  const d = new Date();
  d.setDate(d.getDate() + offsetDays);
  return toLocalInputValue(d).slice(0, 10);
}
