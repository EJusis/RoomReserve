import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { ToggleGroup, ToggleGroupItem } from "@/components/ui/toggle-group";
import { FilterField } from "@/components/FilterField";
import type { Equipment } from "@/api/types";
import { ALL, EQUIPMENT_LABELS, EQUIPMENT_OPTIONS } from "@/lib/labels";
import { ALL_ROOMS, type RoomFilter } from "./api";

interface Props {
  value: RoomFilter;
  floors: number[];
  onChange: (filter: RoomFilter) => void;
}

const toNumber = (text: string) =>
  text === "" || text === ALL ? undefined : Number(text);

export function RoomFilters({ value, floors, onChange }: Props) {
  return (
    <Card className="mb-4 py-4">
      <CardContent className="flex flex-wrap items-end gap-4 px-4">
        <FilterField
          label="Min. capacity"
          htmlFor="min-capacity"
        >
          <Input
            id="min-capacity"
            type="number"
            min={1}
            placeholder="Any"
            className="w-32"
            value={value.minCapacity ?? ""}
            onChange={(e) =>
              onChange({ ...value, minCapacity: toNumber(e.target.value) })
            }
          />
        </FilterField>

        <FilterField label="Floor">
          <Select
            value={value.floor?.toString() ?? ALL}
            onValueChange={(v) => onChange({ ...value, floor: toNumber(v) })}
          >
            <SelectTrigger
              className="w-36"
              aria-label="Floor"
            >
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={ALL}>All floors</SelectItem>
              {floors.map((floor) => (
                <SelectItem
                  key={floor}
                  value={floor.toString()}
                >
                  Floor {floor}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </FilterField>

        <FilterField label="Must have">
          <ToggleGroup
            type="multiple"
            variant="outline"
            value={value.equipment}
            onValueChange={(equipment) =>
              onChange({ ...value, equipment: equipment as Equipment[] })
            }
          >
            {EQUIPMENT_OPTIONS.map((item) => (
              <ToggleGroupItem
                key={item}
                value={item}
                className="flex-none px-3 data-[state=on]:bg-primary data-[state=on]:text-primary-foreground"
              >
                {EQUIPMENT_LABELS[item]}
              </ToggleGroupItem>
            ))}
          </ToggleGroup>
        </FilterField>

        <Button
          variant="ghost"
          className="ml-auto"
          onClick={() => onChange(ALL_ROOMS)}
        >
          Clear filters
        </Button>
      </CardContent>
    </Card>
  );
}
