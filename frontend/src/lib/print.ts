/**
 * Opens the browser's print dialog ("Save as PDF" is one of the destinations).
 * The tab title is used as the suggested file name, so it's swapped for the duration.
 */
export function printPage(fileName: string) {
  const originalTitle = document.title;
  document.title = fileName;

  window.addEventListener(
    "afterprint",
    () => (document.title = originalTitle),
    { once: true },
  );
  window.print();
}
