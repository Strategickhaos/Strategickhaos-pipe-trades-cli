FROM python:3.11-slim
WORKDIR /app
COPY main.py .
ENTRYPOINT ["python", "main.py"]
CMD ["--help"]
