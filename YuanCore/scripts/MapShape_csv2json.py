import csv
import json
import argparse

def convert_csv_to_json(input_csv_path, output_json_path):
    data = []

    with open(input_csv_path, 'r', encoding='utf-8-sig', newline='') as csvfile:
        reader = csv.DictReader(csvfile)

        for row in reader:
            min_x = int(row['MinX'])
            min_y = int(row['MinY'])
            max_x = int(row['MaxX'])
            max_y = int(row['MaxY'])

            item = {
                "Class": row['Class'],
                "Index": int(row['Index']),
                "MinX": min_x,
                "MinY": min_y,
                "Width": max_x - min_x,
                "Height": max_y - min_y
            }
            data.append(item)

    with open(output_json_path, 'w', encoding='utf-8') as jsonfile:
        json.dump(data, jsonfile, indent=2, ensure_ascii=False)

def main():
    parser = argparse.ArgumentParser(
        description="Convert MapShape TSV/CSV to formatted JSON."
    )
    parser.add_argument("input_csv", help="Path to input CSV")
    parser.add_argument("output_json", help="Path to output JSON")
    args = parser.parse_args()

    convert_csv_to_json(args.input_csv, args.output_json)

if __name__ == "__main__":
    main()
