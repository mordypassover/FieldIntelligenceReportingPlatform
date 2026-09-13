import json
import os
from pathlib import Path
from confluent_kafka import Producer
from logger import get_logger

# Initialize logger
logger = get_logger()


def load_data():
    logger.info("Searching for field_reports.json...")
    files = list(Path("/").rglob("field_reports.json"))

    if not files:
        logger.error("Could not find field_reports.json in filesystem!")
        raise FileNotFoundError("field_reports.json not found")

    file_path = files[0]
    logger.info(f"Loading data from: {file_path}")

    with open(file_path, "r", encoding="utf-8") as f:
        data = json.load(f)

    logger.info(f"Successfully loaded {len(data)} reports.")
    return data

def run_producer():
    bootstrap_servers = os.getenv("KAFKA_BOOTSTRAP_SERVERS", "live-streame-kafka:9092")
    logger.info(f"Initializing Kafka producer connecting to {bootstrap_servers}")

    try:
        producer = Producer({"bootstrap.servers": bootstrap_servers})
        data = load_data()

        logger.info("Starting to publish messages...")
        for i, report in enumerate(data):
            producer.produce(
                topic="raw-data",
                key=str(i),
                value=json.dumps(report)
            )
            producer.poll(0)

        logger.info("Flushing remaining messages...")
        producer.flush()
        logger.info("All messages successfully sent!")

    except Exception as e:
        logger.error(f"An error occurred during publishing: {e}")
        raise


if __name__ == "__main__":
    run_producer()