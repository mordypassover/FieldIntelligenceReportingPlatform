FROM python:3.12

WORKDIR /app

ENV KAFKA_BOOTSTRAP_SERVERS="live-streame-kafka:9092" \
    LOG_LEVEL="INFO"

COPY requirements.txt .

RUN pip install --no-cache-dir -r requirements.txt

COPY . .

CMD ["python", "main.py"]