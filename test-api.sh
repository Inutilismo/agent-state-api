#!/bin/bash

API_URL="http://localhost:5000"

echo "🧪 Testing Agent State API..."

echo "1️⃣ Testing CALL_STARTED event..."
response=$(curl -s -w "%{http_code}" -X POST "${API_URL}/api/agentstate/events" \
  -H "Content-Type: application/json" \
  -d '{
    "agentId": "550e8400-e29b-41d4-a716-446655440000",
    "agentName": "test agent",
    "timestampUtc": "'$(date -u +%Y-%m-%dT%H:%M:%S.%3NZ)'",
    "action": "CALL_STARTED",
    "queueIds": ["11111111-1111-1111-1111-111111111111"]
  }')

if [[ "${response: -3}" == "200" ]]; then
    echo "   ✅ CALL_STARTED processed successfully"
else
    echo "   ❌ CALL_STARTED failed (${response: -3})"
    echo "   Response: ${response%???}"
fi

echo "2️⃣ Testing late event validation..."
response=$(curl -s -w "%{http_code}" -X POST "${API_URL}/api/agentstate/events" \
  -H "Content-Type: application/json" \
  -d '{
    "agentId": "550e8400-e29b-41d4-a716-446655440001",
    "agentName": "late agent",
    "timestampUtc": "'$(date -u -d '2 hours ago' +%Y-%m-%dT%H:%M:%S.%3NZ)'",
    "action": "CALL_STARTED",
    "queueIds": []
  }')

if [[ "${response: -3}" == "400" ]]; then
    echo "   ✅ Late event correctly rejected"
else
    echo "   ❌ Late event should have been rejected (${response: -3})"
fi

echo ""
echo "🎉 API testing completed!"
echo "📊 View Swagger UI: ${API_URL}"
