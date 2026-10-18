const hoursFormat = new Intl.NumberFormat("en-GB", {
  maximumFractionDigits: 1,
});
const percentFormat = new Intl.NumberFormat("en-GB", {
  style: "percent",
  maximumFractionDigits: 0,
});

export const formatHours = (hours: number) => `${hoursFormat.format(hours)} h`;
export const formatPercent = (ratio: number) => percentFormat.format(ratio);
