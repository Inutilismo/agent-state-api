#!/bin/bash

echo "🚀 Starting Agent State API with Docker Compose..."

docker compose up --build -d

echo "✅ API is running at: http://localhost:5000"
echo "📊 Swagger UI: http://localhost:5000"
echo "🗄️  PostgreSQL: localhost:5432"
echo ""
echo "📝 To view logs: docker compose logs -f"
echo "🛑 To stop: docker compose down"
