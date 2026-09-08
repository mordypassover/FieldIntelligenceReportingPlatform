import json
from pathlib import Path
from confluent_kafka import Producer


file_path = (
    list(
        Path(
            r"C:/Users/mordy/My_Projects/micro_services/FieldIntelligenceReportingPlatform"
        )
        .rglob("field_reports.json"
        )
    )
)

with open(file_path[0], "r",encoding="utf-8") as f:
    data = json.load(f)

config = {'bootstrap.servers': 'localhost:9092'}
producer = Producer(config)

for i, report in enumerate(data):
    producer.produce("raw-data", key=str(i), value=json.dumps(report))

producer.flush()


