import csv
import json
import argparse
from collections import defaultdict


def normalize_int(value):
    """Convert CSV value to int safely."""
    return int(str(value).strip())


def read_csv_rows(csv_path):
    rows = []
    with open(csv_path, "r", encoding="utf-8-sig", newline="") as f:
        reader = csv.DictReader(f)
        required_columns = {
            "buildClassID",
            "buildStateID",
            "minA",
            "minB",
            "maxA",
            "maxB",
        }
        missing = required_columns - set(reader.fieldnames or [])
        if missing:
            raise ValueError(f"CSV missing required columns: {sorted(missing)}")

        for line_no, row in enumerate(reader, start=2):
            try:
                rows.append(
                    {
                        "buildClassID": normalize_int(row["buildClassID"]),
                        "buildStateID": normalize_int(row["buildStateID"]),
                        "minA": normalize_int(row["minA"]),
                        "minB": normalize_int(row["minB"]),
                        "maxA": normalize_int(row["maxA"]),
                        "maxB": normalize_int(row["maxB"]),
                    }
                )
            except Exception as e:
                raise ValueError(f"Failed to parse line {line_no}: {e}") from e
    return rows


def is_swap_rotation(prev_row, next_row):
    """
    Check the rotation rule:
    (minA, minB, maxA, maxB) -> (minB, minA, maxB, maxA)
    """
    return (
        next_row["minA"] == prev_row["minB"]
        and next_row["minB"] == prev_row["minA"]
        and next_row["maxA"] == prev_row["maxB"]
        and next_row["maxB"] == prev_row["maxA"]
    )


def group_by_building(rows):
    grouped = defaultdict(list)
    for row in rows:
        grouped[row["buildClassID"]].append(row)

    # Sort states inside each building
    for build_id in grouped:
        grouped[build_id].sort(key=lambda r: r["buildStateID"])

    return grouped


def building_follows_rule(state_rows):
    """
    Decide whether a building can be merged into one JSON object.

    Rules used here:
    1. If only one state exists, treat it as mergeable (single object).
    2. For multiple states, every adjacent pair in sorted buildStateID order
       must satisfy the swap rule.
    3. If the states are exactly [0, 1, 2, 3], also verify 3 -> 0 cyclically.
    """
    if len(state_rows) <= 1:
        return True

    for i in range(len(state_rows) - 1):
        if not is_swap_rotation(state_rows[i], state_rows[i + 1]):
            return False

    state_ids = [r["buildStateID"] for r in state_rows]
    if state_ids == [0, 1, 2, 3]:
        if not is_swap_rotation(state_rows[-1], state_rows[0]):
            return False

    return True


def make_footprint(row):
    return {
        "XMin": row["minA"],
        "YMin": row["minB"],
        "XMax": row["maxA"],
        "YMax": row["maxB"],
        "Layer": "MainBuilding",
    }


def convert_to_json_objects(rows):
    grouped = group_by_building(rows)
    result = []

    for build_id in sorted(grouped.keys()):
        state_rows = grouped[build_id]
        follows_rule = building_follows_rule(state_rows)

        if follows_rule:
            base_row = min(state_rows, key=lambda r: r["buildStateID"])
            result.append(
                {
                    "Id": build_id,
                    "Rotations": [r["buildStateID"] for r in state_rows],
                    "Footprints": [make_footprint(base_row)],
                }
            )
        else:
            for row in state_rows:
                result.append(
                    {
                        "Id": build_id,
                        "Rotations": [row["buildStateID"]],
                        "Footprints": [make_footprint(row)],
                    }
                )

    return result


def main():
    parser = argparse.ArgumentParser(
        description="Convert building rotation CSV to formatted JSON."
    )
    parser.add_argument("input_csv", help="Path to input CSV")
    parser.add_argument("output_json", help="Path to output JSON")
    args = parser.parse_args()

    rows = read_csv_rows(args.input_csv)
    json_objects = convert_to_json_objects(rows)

    with open(args.output_json, "w", encoding="utf-8") as f:
        json.dump(json_objects, f, ensure_ascii=False, indent=2)

    print(f"Done. Wrote {len(json_objects)} JSON objects to {args.output_json}")


if __name__ == "__main__":
    main()
