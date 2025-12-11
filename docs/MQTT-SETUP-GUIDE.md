# MQTT Machine Log Ingestion Setup Guide

This guide explains how to set up and test the MQTT machine log ingestion system for FLOWerTRACK.

## Overview

The system consists of:

1. **MQTT Broker** - Message broker (e.g., Mosquitto, HiveMQ, EMQ X)
2. **Backend Service** - ASP.NET Core API with MQTT ingestion
3. **Test Simulator** - Console application for generating test logs

## Architecture

```
┌─────────────────┐       MQTT        ┌──────────────────┐
│   Machines      │  ───────────────> │   MQTT Broker    │
│  (Simulator)    │   machines/       │  (Mosquitto)     │
│                 │   {token}/logs    │                  │
└─────────────────┘                   └──────────────────┘
                                               │
                                               │ Subscribe
                                               │ machines/+/logs
                                               ▼
                                      ┌──────────────────┐
                                      │  FLOWerTRACK API │
                                      │  MQTT Ingestion  │
                                      │  Service         │
                                      └──────────────────┘
                                               │
                                        Worker Pool (10 workers)
                                               │
                                        ┌──────┴──────┐
                                        ▼             ▼
                                   ┌─────────┐  ┌─────────┐
                                   │ Worker 1│  │ Worker N│
                                   └─────────┘  └─────────┘
                                        │             │
                                        └──────┬──────┘
                                               ▼
                                      ┌──────────────────┐
                                      │   PostgreSQL     │
                                      │   Database       │
                                      │  (machine_logs)  │
                                      └──────────────────┘
```

## Prerequisites

- .NET 10.0 SDK
- PostgreSQL database
- MQTT Broker (Mosquitto recommended for testing)
- Docker (optional, for containerized broker)

## Step 1: Install MQTT Broker

### Option A: Docker (Recommended for Testing)

```bash
# Pull and run Mosquitto MQTT broker
docker run -d \
  --name mosquitto \
  -p 1883:1883 \
  -p 9001:9001 \
  eclipse-mosquitto:latest
```

### Option B: Native Installation (Linux/macOS)

```bash
# Ubuntu/Debian
sudo apt-get update
sudo apt-get install mosquitto mosquitto-clients

# macOS (Homebrew)
brew install mosquitto
brew services start mosquitto

# Test connection
mosquitto_sub -t "test/#" -v
```

### Option C: Windows

Download and install from: https://mosquitto.org/download/

Or use Docker Desktop with the command from Option A.

## Step 2: Configure Backend

Update the `appsettings.Development.json` or use user secrets:

```bash
cd src/backend/Presentation/Flowertrack.Api

# Configure MQTT settings
dotnet user-secrets set "Mqtt:Enabled" "true"
dotnet user-secrets set "Mqtt:Server" "localhost"
dotnet user-secrets set "Mqtt:Port" "1883"
dotnet user-secrets set "Mqtt:TopicPattern" "machines/+/logs"
dotnet user-secrets set "Mqtt:WorkerCount" "10"
dotnet user-secrets set "Mqtt:QueueCapacity" "10000"
```

Or edit `appsettings.Development.json`:

```json
{
  "Mqtt": {
    "Enabled": true,
    "Server": "localhost",
    "Port": 1883,
    "Username": "",
    "Password": "",
    "ClientId": "",
    "UseTls": false,
    "TopicPattern": "machines/+/logs",
    "WorkerCount": 10,
    "QueueCapacity": 10000,
    "MaxRetries": 3,
    "RetryDelaySeconds": 5,
    "ConnectionTimeoutSeconds": 30,
    "KeepAliveSeconds": 60,
    "MaxLogsPerMinutePerMachine": 200,
    "OverloadThresholdPercent": 80
  }
}
```

## Step 3: Prepare Database

Ensure PostgreSQL is running and the database exists:

```bash
# Create database (if not exists)
createdb flowertrack_dev

# Apply migrations
cd src/backend/Presentation/Flowertrack.Api
dotnet ef database update
```

## Step 4: Generate Machine Tokens

Create test machines with API tokens. You can use the API or insert directly:

```sql
-- Example: Insert test machine
INSERT INTO machines (
  id, 
  organization_id, 
  serial_number, 
  brand, 
  model, 
  status, 
  api_token,
  created_at
) VALUES (
  gen_random_uuid(),
  '00000000-0000-0000-0000-000000000001', -- Replace with actual org ID
  'TEST-MACHINE-001',
  'Baumalog',
  'Model-X',
  'Inactive',
  'aBcDeFgH123M', -- 12-character token
  NOW()
);
```

Or generate tokens programmatically using the `MachineApiKey.Generate()` method.

## Step 5: Start Backend Service

```bash
cd src/backend/Presentation/Flowertrack.Api
dotnet run
```

Watch the logs for:
```
[XX:XX:XX INF] MQTT Log Ingestion Service created with 10 workers and queue capacity 10000
[XX:XX:XX INF] Starting MQTT Log Ingestion Service - connecting to localhost:1883
[XX:XX:XX INF] Successfully connected to MQTT broker
[XX:XX:XX INF] Successfully subscribed to topic: machines/+/logs with QoS: GrantedQoS1
[XX:XX:XX INF] Started worker 1
[XX:XX:XX INF] Started worker 2
...
```

## Step 6: Run Test Simulator

### Basic Test (50 machines, 500 logs/min)

```bash
cd src/backend/Tools/MqttLogSimulator/Flowertrack.MqttLogSimulator
dotnet run -- -m 50 -r 500 -d 120
```

### Requirements Validation Test

Test backend requirement (10,000+ logs/hour from 50+ machines):

```bash
# 10,000 logs per hour = ~167 logs/minute
dotnet run -- -m 50 -r 167 -d 3600
```

Expected output:
```
╔══════════════════════════════════════════════════════════════╗
║        FLOWerTRACK MQTT Log Simulator                        ║
║        Machine Log Testing Tool v1.0                         ║
╚══════════════════════════════════════════════════════════════╝

Configuration:
  MQTT Broker:        localhost:1883
  Number of Machines: 50
  Logs per Minute:    167
  Duration:           3600s
  Burst Mode:         No

Generated 50 machine tokens
Connecting to MQTT broker... Connected!

Starting continuous log generation...
Press CTRL+C to stop

[20:45:15] Stats: 14 sent, 0 failed, 14 logs/min, 15.3ms avg, 1 with alarms
[20:45:20] Stats: 14 sent, 0 failed, 14 logs/min, 12.8ms avg, 0 with alarms
...
```

### Burst Mode Test

Test spike handling (10,000 logs in 30 seconds):

```bash
dotnet run -- -m 80 --burst 10000 30
```

### High Throughput Test

Test maximum sustained throughput:

```bash
dotnet run -- -m 100 -r 2000 -d 600
```

## Monitoring and Validation

### Check Backend Logs

Watch for successful processing:

```
[XX:XX:XX INF] Successfully processed log for machine {MachineId} (Token: {Token}), 
               Cycles: {Cycles}, Active Alarms: 0, New Alarms: 0
```

Watch for warnings:

```
[XX:XX:XX WRN] System is overloaded - Queue: 8500/10000 (85%), Messages/min: 2100
[XX:XX:XX WRN] Rate limit exceeded for machine {Token}, dropping message
```

### Check Database

Verify logs are being persisted:

```sql
-- Count logs received
SELECT COUNT(*) FROM machine_logs;

-- Recent logs by machine
SELECT 
  m.serial_number,
  COUNT(ml.id) as log_count,
  MAX(ml.received_at) as last_log
FROM machine_logs ml
JOIN machines m ON ml.machine_id = m.id
WHERE ml.received_at > NOW() - INTERVAL '1 hour'
GROUP BY m.serial_number
ORDER BY log_count DESC;

-- Logs with alarms
SELECT 
  COUNT(*) as alarm_logs,
  COUNT(*) * 100.0 / (SELECT COUNT(*) FROM machine_logs) as alarm_percentage
FROM machine_logs 
WHERE alarm_code IS NOT NULL;
```

### API Endpoints for Monitoring

The MQTT service exposes statistics via health checks:

```bash
# Check overall health
curl http://localhost:5001/health

# Check MQTT connection specifically
curl http://localhost:5001/health/mqtt
```

## Testing Scenarios

### Scenario 1: Baseline Performance (Pass Criteria)

**Test:**
```bash
dotnet run -- -m 50 -r 500 -d 600
```

**Expected Results:**
- ✓ All logs successfully delivered (success rate ≥ 99%)
- ✓ Average latency < 100ms
- ✓ No queue overflow warnings
- ✓ Database contains all expected logs
- ✓ Worker distribution is balanced

**Validation:**
```sql
-- Should be approximately 5,000 logs (500/min × 10 min)
SELECT COUNT(*) FROM machine_logs 
WHERE received_at > NOW() - INTERVAL '10 minutes';
```

### Scenario 2: Requirements Validation (Pass Criteria)

**Test:**
```bash
dotnet run -- -m 50 -r 167 -d 3600
```

**Expected Results:**
- ✓ ≥10,000 logs successfully processed in 1 hour
- ✓ Data from all 50+ machines
- ✓ No message loss
- ✓ System stability maintained

### Scenario 3: Burst Handling (Pass Criteria)

**Test:**
```bash
dotnet run -- -m 80 --burst 10000 30
```

**Expected Results:**
- ✓ System buffers traffic effectively
- ✓ Message loss < 1%
- ✓ Queue doesn't exceed 90% capacity
- ✓ All logs eventually persisted
- ✓ No system crashes

### Scenario 4: Overload Detection (Pass Criteria)

**Test:**
```bash
dotnet run -- -m 150 -r 5000 -d 300
```

**Expected Results:**
- ✓ Overload warnings logged when queue > 80%
- ✓ Rate limiting activates for machines exceeding 200 logs/min
- ✓ System remains stable
- ✓ No database errors
- ✓ Graceful degradation

## Troubleshooting

### Problem: Backend can't connect to MQTT broker

**Solution:**
```bash
# Test broker is running
docker ps | grep mosquitto

# Test broker connectivity
telnet localhost 1883

# Check broker logs
docker logs mosquitto
```

### Problem: Logs not appearing in database

**Solution:**
```bash
# Check backend logs for errors
grep -i "error\|exception" logs/flowertrack-*.log

# Verify machine tokens exist
SELECT id, serial_number, api_token FROM machines;

# Check MQTT ingestion service started
grep "MQTT Log Ingestion" logs/flowertrack-*.log
```

### Problem: High failure rate in simulator

**Solution:**
- Reduce rate: `-r 100`
- Reduce machine count: `-m 20`
- Check network connectivity
- Verify broker capacity limits

### Problem: Backend performance degradation

**Solution:**
- Check PostgreSQL performance
- Verify worker count is appropriate
- Check system resources (CPU, memory)
- Review queue capacity settings

## Performance Tuning

### Adjust Worker Count

Based on CPU cores:
```json
{
  "Mqtt": {
    "WorkerCount": 20  // Increase for more parallel processing
  }
}
```

### Adjust Queue Capacity

For higher burst tolerance:
```json
{
  "Mqtt": {
    "QueueCapacity": 20000  // Increase for more buffering
  }
}
```

### Adjust Rate Limiting

Per-machine throughput limit:
```json
{
  "Mqtt": {
    "MaxLogsPerMinutePerMachine": 300  // Adjust based on needs
  }
}
```

## Production Considerations

1. **Use TLS/SSL** for MQTT connections
2. **Enable authentication** on MQTT broker
3. **Set up monitoring** (Prometheus, Grafana)
4. **Configure log retention** policies
5. **Scale horizontally** with multiple API instances
6. **Use managed MQTT** service (AWS IoT Core, Azure IoT Hub)
7. **Implement circuit breakers** for database connections
8. **Set up alerting** for overload conditions

## Security Checklist

- [ ] MQTT broker authentication enabled
- [ ] TLS/SSL enabled for MQTT connections
- [ ] Machine tokens properly validated
- [ ] Rate limiting configured
- [ ] Database access restricted
- [ ] Logs sanitized for sensitive data
- [ ] Connection strings stored securely (user secrets, vault)

## Next Steps

After successful testing:

1. Configure production MQTT broker
2. Generate real machine tokens
3. Deploy backend to production
4. Configure monitoring and alerting
5. Set up log retention policies
6. Document operational procedures

## Support

For issues or questions:
- Review backend logs: `logs/flowertrack-*.log`
- Check database connectivity
- Verify MQTT broker status
- Consult main repository documentation
