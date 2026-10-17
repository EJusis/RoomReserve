import { AlertCircleIcon } from "lucide-react";
import { getErrorMessages } from "@/api/client";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";

export function ErrorAlert({
  error,
  title = "Something went wrong",
}: {
  error: unknown;
  title?: string;
}) {
  if (!error) return null;
  const messages = getErrorMessages(error);

  return (
    <Alert variant="destructive">
      <AlertCircleIcon />
      <AlertTitle>{title}</AlertTitle>
      <AlertDescription>
        {messages.length === 1 ? (
          <p>{messages[0]}</p>
        ) : (
          <ul className="list-disc pl-4">
            {messages.map((m) => (
              <li key={m}>{m}</li>
            ))}
          </ul>
        )}
      </AlertDescription>
    </Alert>
  );
}
