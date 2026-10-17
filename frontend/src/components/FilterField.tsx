import type { ReactNode } from "react";
import { Label } from "@/components/ui/label";

export function FilterField({
  label,
  htmlFor,
  children,
}: {
  label: string;
  htmlFor?: string;
  children: ReactNode;
}) {
  return (
    <div className="grid gap-1.5">
      <Label
        htmlFor={htmlFor}
        className="text-muted-foreground text-xs"
      >
        {label}
      </Label>
      {children}
    </div>
  );
}
