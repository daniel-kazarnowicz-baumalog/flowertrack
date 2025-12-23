# FLOWerTRACK MQTT Log Simulator

A comprehensive testing tool for simulating machine logs via MQTT protocol. This simulator generates realistic machine telemetry data to test the FLOWerTRACK log ingestion backend under various load conditions.

## Features

- **Multi-Machine Simulation**: Simulates up to hundreds of machines simultaneously
- **Configurable Rate Control**: Adjust log generation rate (logs per minute)
- **Burst Mode Testing**: Generate high-volume spike traffic to test system limits
- **Realistic Data Generation**: 
  - Random alarm generation and clearing
  - Temperature fluctuations
  - Cycle counting
  - Operator assignment
  - Custom telemetry data
- **Real-time Statistics**: Live monitoring of sent logs, success rate, and performance
- **Rate Limiting Simulation**: Tests backend rate limiting capabilities

## Requirements

- .NET 10.0 SDK
- Access to an MQTT broker (e.g., Mosquitto, HiveMQ, EMQ X)

## Installation

```bash
cd src/backend/Tools/MqttLogSimulator/Flowertrack.MqttLogSimulator
dotnet restore
dotnet build
```

## Usage

### Interactive Mode

Simply run without arguments for interactive configuration:

```bash
dotnet run
```

You'll be prompted to enter:
- MQTT Broker Address (default: localhost)
- MQTT Broker Port (default: 1883)
- Number of Machines (default: 80)
- Logs per Minute (default: 500)
- Duration in seconds (0 for continuous)
- Burst mode option

### Command Line Mode

Run with arguments for automated testing:

```bash
dotnet run -- [options]
```

#### Options

- `-b, --broker <address>` - MQTT broker address (default: localhost)
- `-p, --port <port>` - MQTT broker port (default: 1883)
- `-m, --machines <count>` - Number of machines to simulate (default: 80)
- `-r, --rate <logs/min>` - Logs per minute (default: 500)
- `-d, --duration <seconds>` - Duration in seconds, 0 = continuous (default: 0)
- `--burst <count> <seconds>` - Burst mode: send <count> logs in <seconds>
- `-h, --help` - Show help message

## Examples

### Basic Continuous Testing

Test with 50 machines sending 1000 logs per minute continuously:

```bash
dotnet run -- -m 50 -r 1000
```

### Duration-Limited Test

Run for 5 minutes (300 seconds):

```bash
dotnet run -- -m 50 -r 1000 -d 300
```

### Burst Mode Testing

High-intensity burst: 10,000 logs in 30 seconds from 80 machines:

```bash
dotnet run -- -m 80 --burst 10000 30
```

### Production-Scale Test

Simulate 100 machines with high throughput (2000 logs/min):

```bash
dotnet run -- -b mqtt.example.com -p 1883 -m 100 -r 2000 -d 600
```

### Requirements Validation Test

Test backend requirements (10,000+ logs/hour from 50+ machines):

```bash
# 10,000 logs per hour = ~167 logs/minute
dotnet run -- -m 50 -r 167 -d 3600
```

## Message Format

The simulator generates messages matching the backend's expected format:

```json
{
  "token": "aBcDeFgH123",
  "timestamp": "2025-12-11T20:45:00Z",
  "cycles": 12345,
  "active_alarms": [201, 202],
  "new_alarms": [202],
  "operator_id": 172,
  "temperature": 38.7,
  "custom_data": {
    "pressure": 150,
    "humidity": 55,
    "speed": 100
  }
}
```

## Statistics and Monitoring

The simulator provides real-time statistics every 5 seconds:

- **Logs Sent**: Successfully published messages
- **Logs Failed**: Failed publish attempts
- **Logs/Min**: Current throughput
- **Average Duration**: Average publish time in milliseconds
- **Alarms**: Count of logs containing new alarms

### Final Report

After completion, a comprehensive report displays:
- Total logs sent/failed
- Duration statistics (min/max/average)
- Alarm distribution
- Top 10 machines by log count
- Overall alarm rate

## Testing Scenarios

### Scenario 1: Baseline Performance

Test normal operational load:

```bash
dotnet run -- -m 50 -r 500 -d 600
```

**Expected Results:**
- All logs successfully delivered
- Average latency < 100ms
- No message loss

### Scenario 2: Peak Load

Test maximum sustained throughput:

```bash
dotnet run -- -m 80 -r 2000 -d 300
```

**Expected Results:**
- Backend maintains data integrity
- Worker pool handles load distribution
- Queue doesn't overflow

### Scenario 3: Burst Traffic

Test spike handling capabilities:

```bash
dotnet run -- -m 80 --burst 10000 30
```

**Expected Results:**
- System buffers traffic effectively
- No message loss
- Graceful degradation if overloaded

### Scenario 4: Long-Duration Stability

Test system stability over time:

```bash
dotnet run -- -m 100 -r 1000 -d 7200
```

**Expected Results:**
- Consistent performance over 2 hours
- No memory leaks
- Stable connection

## MQTT Topics

The simulator publishes to topics in the format:

```
machines/{token}/logs
```

Example: `machines/aBcDeFgH123/logs`

The backend subscribes using wildcard: `machines/+/logs`

## Troubleshooting

### Connection Refused

- Ensure MQTT broker is running
- Verify broker address and port
- Check firewall rules

### High Failure Rate

- Check broker capacity limits
- Verify network stability
- Review backend logs for processing errors

### Performance Issues

- Reduce rate or machine count
- Check system resources (CPU, memory, network)
- Review broker and backend logs

## Integration with Backend

The simulator works with the FLOWerTRACK backend's MQTT ingestion system:

1. Backend subscribes to `machines/+/logs`
2. Simulator publishes to `machines/{token}/logs`
3. Backend validates token and processes log
4. Backend persists to database before sending MQTT ACK
5. Worker pool distributes load across multiple processors

## License

This tool is part of the FLOWerTRACK project and follows the same license.

## Support

For issues or questions, please refer to the main FLOWerTRACK repository documentation.
