using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace Flowertrack.MqttLogSimulator;

/// <summary>
/// MQTT Machine Log Simulator
/// Generates random machine logs and publishes them via MQTT to test the log ingestion backend
/// </summary>
class Program
{
    private static readonly Random Random = new();
    private static readonly ConcurrentDictionary<string, MachineState> MachineStates = new();
    private static readonly ConcurrentQueue<LogStatistic> LogStatistics = new();
    private static IMqttClient? _mqttClient;
    private static SimulatorConfig _config = new();
    
    static async Task Main(string[] args)
    {
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║        FLOWerTRACK MQTT Log Simulator                        ║");
        Console.WriteLine("║        Machine Log Testing Tool v1.0                         ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        // Parse command line arguments or use interactive mode
        if (args.Length > 0)
        {
            ParseArguments(args);
        }
        else
        {
            ConfigureInteractive();
        }

        Console.WriteLine();
        Console.WriteLine("Configuration:");
        Console.WriteLine($"  MQTT Broker:        {_config.BrokerAddress}:{_config.BrokerPort}");
        Console.WriteLine($"  Number of Machines: {_config.MachineCount}");
        Console.WriteLine($"  Logs per Minute:    {_config.LogsPerMinute}");
        Console.WriteLine($"  Duration:           {(_config.DurationSeconds == 0 ? "Continuous" : $"{_config.DurationSeconds}s")}");
        Console.WriteLine($"  Burst Mode:         {(_config.BurstMode ? "Yes" : "No")}");
        if (_config.BurstMode)
        {
            Console.WriteLine($"  Burst Count:        {_config.BurstLogCount} logs in {_config.BurstDurationSeconds}s");
        }
        Console.WriteLine();

        try
        {
            // Generate machine tokens
            var machineTokens = GenerateMachineTokens(_config.MachineCount);
            Console.WriteLine($"Generated {machineTokens.Count} machine tokens");

            // Connect to MQTT broker
            await ConnectToMqttBroker();

            // Start statistics display
            var statsCts = new CancellationTokenSource();
            var statsTask = Task.Run(() => DisplayStatistics(statsCts.Token));

            // Start log generation
            if (_config.BurstMode)
            {
                await RunBurstMode(machineTokens);
            }
            else
            {
                await RunContinuousMode(machineTokens);
            }

            // Stop statistics display
            await statsCts.CancelAsync();
            await statsTask;

            // Disconnect
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
            }

            Console.WriteLine();
            Console.WriteLine("Simulation completed successfully!");
            DisplayFinalStatistics();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
            Environment.Exit(1);
        }
    }

    private static void ParseArguments(string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--broker":
                case "-b":
                    if (i + 1 < args.Length)
                        _config.BrokerAddress = args[++i];
                    break;
                case "--port":
                case "-p":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out var port))
                        _config.BrokerPort = port;
                    break;
                case "--machines":
                case "-m":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out var count))
                        _config.MachineCount = count;
                    break;
                case "--rate":
                case "-r":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out var rate))
                        _config.LogsPerMinute = rate;
                    break;
                case "--duration":
                case "-d":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out var duration))
                        _config.DurationSeconds = duration;
                    break;
                case "--burst":
                    _config.BurstMode = true;
                    if (i + 1 < args.Length && int.TryParse(args[++i], out var burstCount))
                        _config.BurstLogCount = burstCount;
                    if (i + 1 < args.Length && int.TryParse(args[++i], out var burstDuration))
                        _config.BurstDurationSeconds = burstDuration;
                    break;
                case "--help":
                case "-h":
                    ShowHelp();
                    Environment.Exit(0);
                    break;
            }
        }
    }

    private static void ConfigureInteractive()
    {
        Console.Write("MQTT Broker Address [localhost]: ");
        var broker = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(broker))
            _config.BrokerAddress = broker;

        Console.Write("MQTT Broker Port [1883]: ");
        if (int.TryParse(Console.ReadLine(), out var port))
            _config.BrokerPort = port;

        Console.Write("Number of Machines [80]: ");
        if (int.TryParse(Console.ReadLine(), out var machineCount))
            _config.MachineCount = machineCount;

        Console.Write("Logs per Minute [500]: ");
        if (int.TryParse(Console.ReadLine(), out var rate))
            _config.LogsPerMinute = rate;

        Console.Write("Duration in Seconds (0 for continuous) [0]: ");
        if (int.TryParse(Console.ReadLine(), out var duration))
            _config.DurationSeconds = duration;

        Console.Write("Enable Burst Mode? (y/n) [n]: ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            _config.BurstMode = true;
            Console.Write("Burst Log Count [10000]: ");
            if (int.TryParse(Console.ReadLine(), out var burstCount))
                _config.BurstLogCount = burstCount;

            Console.Write("Burst Duration in Seconds [30]: ");
            if (int.TryParse(Console.ReadLine(), out var burstDuration))
                _config.BurstDurationSeconds = burstDuration;
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Usage: Flowertrack.MqttLogSimulator [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -b, --broker <address>     MQTT broker address (default: localhost)");
        Console.WriteLine("  -p, --port <port>          MQTT broker port (default: 1883)");
        Console.WriteLine("  -m, --machines <count>     Number of machines to simulate (default: 80)");
        Console.WriteLine("  -r, --rate <logs/min>      Logs per minute (default: 500)");
        Console.WriteLine("  -d, --duration <seconds>   Duration in seconds (0 = continuous) (default: 0)");
        Console.WriteLine("  --burst <count> <seconds>  Burst mode: send <count> logs in <seconds>");
        Console.WriteLine("  -h, --help                 Show this help message");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  Interactive mode:");
        Console.WriteLine("    dotnet run");
        Console.WriteLine();
        Console.WriteLine("  Continuous mode with 50 machines, 1000 logs/min:");
        Console.WriteLine("    dotnet run -- -m 50 -r 1000");
        Console.WriteLine();
        Console.WriteLine("  Burst mode test (10000 logs in 30 seconds):");
        Console.WriteLine("    dotnet run -- -m 80 --burst 10000 30");
    }

    private static List<string> GenerateMachineTokens(int count)
    {
        var tokens = new List<string>();
        var allowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
        
        for (int i = 0; i < count; i++)
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(12);
            var token = new StringBuilder();
            
            for (int j = 0; j < 12; j++)
            {
                token.Append(allowedChars[tokenBytes[j] % allowedChars.Length]);
            }
            
            var tokenString = token.ToString();
            tokens.Add(tokenString);
            
            // Initialize machine state
            MachineStates[tokenString] = new MachineState
            {
                Token = tokenString,
                Cycles = Random.Next(1000, 100000),
                ActiveAlarms = new List<int>(),
                Temperature = 20 + Random.Next(0, 30)
            };
        }
        
        return tokens;
    }

    private static async Task ConnectToMqttBroker()
    {
        Console.Write("Connecting to MQTT broker... ");
        
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(_config.BrokerAddress, _config.BrokerPort)
            .WithClientId($"flowertrack-simulator-{Guid.NewGuid():N}")
            .WithCleanSession(true)
            .Build();

        await _mqttClient.ConnectAsync(options);
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Connected!");
        Console.ResetColor();
    }

    private static async Task RunContinuousMode(List<string> machineTokens)
    {
        Console.WriteLine();
        Console.WriteLine("Starting continuous log generation...");
        Console.WriteLine("Press CTRL+C to stop");
        Console.WriteLine();

        var stopwatch = Stopwatch.StartNew();
        var logsToSend = _config.LogsPerMinute;
        var intervalMs = 60000 / logsToSend; // milliseconds between logs

        var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        var endTime = _config.DurationSeconds > 0
            ? DateTime.UtcNow.AddSeconds(_config.DurationSeconds)
            : DateTime.MaxValue;

        try
        {
            while (!cts.Token.IsCancellationRequested && DateTime.UtcNow < endTime)
            {
                var machineToken = machineTokens[Random.Next(machineTokens.Count)];
                await SendMachineLog(machineToken);
                
                // Rate limiting
                await Task.Delay(intervalMs, cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine();
            Console.WriteLine("Stopping log generation...");
        }
    }

    private static async Task RunBurstMode(List<string> machineTokens)
    {
        Console.WriteLine();
        Console.WriteLine($"Starting burst mode: {_config.BurstLogCount} logs in {_config.BurstDurationSeconds} seconds...");
        Console.WriteLine();

        var stopwatch = Stopwatch.StartNew();
        var tasks = new List<Task>();
        var intervalMs = (_config.BurstDurationSeconds * 1000.0) / _config.BurstLogCount;

        for (int i = 0; i < _config.BurstLogCount; i++)
        {
            var machineToken = machineTokens[Random.Next(machineTokens.Count)];
            tasks.Add(SendMachineLog(machineToken));
            
            if (intervalMs > 1)
            {
                await Task.Delay((int)intervalMs);
            }

            // Show progress every 1000 logs
            if ((i + 1) % 1000 == 0)
            {
                var progress = ((i + 1) * 100.0) / _config.BurstLogCount;
                var elapsed = stopwatch.Elapsed.TotalSeconds;
                var rate = (i + 1) / elapsed;
                Console.WriteLine($"Progress: {i + 1}/{_config.BurstLogCount} ({progress:F1}%) - Rate: {rate:F0} logs/sec");
            }
        }

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        Console.WriteLine();
        Console.WriteLine($"Burst completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds");
        Console.WriteLine($"Actual rate: {_config.BurstLogCount / stopwatch.Elapsed.TotalSeconds:F0} logs/sec");
    }

    private static async Task SendMachineLog(string machineToken)
    {
        var state = MachineStates[machineToken];
        var startTime = DateTimeOffset.UtcNow;

        try
        {
            // Update machine state
            state.Cycles += Random.Next(1, 10);
            state.Temperature += (decimal)(Random.NextDouble() * 2 - 1); // +/- 1 degree

            // Randomly generate alarms (5% chance)
            var newAlarms = new List<int>();
            if (Random.Next(100) < 5)
            {
                var alarmCode = Random.Next(200, 300);
                if (!state.ActiveAlarms.Contains(alarmCode))
                {
                    state.ActiveAlarms.Add(alarmCode);
                    newAlarms.Add(alarmCode);
                }
            }

            // Randomly clear alarms (10% chance if there are active alarms)
            if (state.ActiveAlarms.Count > 0 && Random.Next(100) < 10)
            {
                var indexToRemove = Random.Next(state.ActiveAlarms.Count);
                state.ActiveAlarms.RemoveAt(indexToRemove);
            }

            // Create log message
            var logMessage = new
            {
                token = machineToken,
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                cycles = state.Cycles,
                active_alarms = state.ActiveAlarms.ToArray(),
                new_alarms = newAlarms.ToArray(),
                operator_id = state.OperatorId,
                temperature = state.Temperature,
                custom_data = new
                {
                    pressure = Random.Next(100, 200),
                    humidity = Random.Next(30, 70),
                    speed = Random.Next(50, 150)
                }
            };

            var json = JsonConvert.SerializeObject(logMessage);
            var topic = $"machines/{machineToken}/logs";

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(json)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                await _mqttClient.PublishAsync(message);

                var elapsed = (DateTimeOffset.UtcNow - startTime).TotalMilliseconds;
                LogStatistics.Enqueue(new LogStatistic
                {
                    Timestamp = DateTimeOffset.UtcNow,
                    MachineToken = machineToken,
                    Success = true,
                    DurationMs = elapsed,
                    HasNewAlarms = newAlarms.Count > 0
                });
            }
        }
        catch (Exception ex)
        {
            var elapsed = (DateTimeOffset.UtcNow - startTime).TotalMilliseconds;
            LogStatistics.Enqueue(new LogStatistic
            {
                Timestamp = DateTimeOffset.UtcNow,
                MachineToken = machineToken,
                Success = false,
                DurationMs = elapsed,
                Error = ex.Message
            });
        }
    }

    private static async Task DisplayStatistics(CancellationToken cancellationToken)
    {
        var lastDisplayTime = DateTimeOffset.UtcNow;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(5000, cancellationToken);
            
            if (cancellationToken.IsCancellationRequested)
                break;

            var now = DateTimeOffset.UtcNow;
            var recentLogs = new List<LogStatistic>();
            
            while (LogStatistics.TryPeek(out var stat) && (now - stat.Timestamp).TotalMinutes < 1)
            {
                if (LogStatistics.TryDequeue(out var dequeuedStat))
                {
                    recentLogs.Add(dequeuedStat);
                }
                else
                {
                    break;
                }
            }

            if (recentLogs.Count > 0)
            {
                var successful = recentLogs.Count(l => l.Success);
                var failed = recentLogs.Count - successful;
                var avgDuration = recentLogs.Any() ? recentLogs.Average(l => l.DurationMs) : 0;
                var logsPerMinute = recentLogs.Count;
                var alarmsCount = recentLogs.Count(l => l.HasNewAlarms);

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Stats: {successful} sent, {failed} failed, " +
                                $"{logsPerMinute} logs/min, {avgDuration:F1}ms avg, {alarmsCount} with alarms");
            }
        }
    }

    private static void DisplayFinalStatistics()
    {
        var allLogs = new List<LogStatistic>();
        while (LogStatistics.TryDequeue(out var stat))
        {
            allLogs.Add(stat);
        }

        if (allLogs.Count == 0)
            return;

        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("                    Final Statistics                           ");
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine($"Total Logs Sent:        {allLogs.Count(l => l.Success)}");
        Console.WriteLine($"Total Logs Failed:      {allLogs.Count(l => !l.Success)}");
        
        if (allLogs.Count > 0)
        {
            Console.WriteLine($"Average Duration:       {allLogs.Average(l => l.DurationMs):F2} ms");
            Console.WriteLine($"Min Duration:           {allLogs.Min(l => l.DurationMs):F2} ms");
            Console.WriteLine($"Max Duration:           {allLogs.Max(l => l.DurationMs):F2} ms");
        }
        
        Console.WriteLine($"Logs with New Alarms:   {allLogs.Count(l => l.HasNewAlarms)}");
        
        // Machine distribution
        var machineDistribution = allLogs
            .GroupBy(l => l.MachineToken)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToList();

        Console.WriteLine();
        Console.WriteLine("Top 10 Machines by Log Count:");
        foreach (var group in machineDistribution)
        {
            Console.WriteLine($"  {group.Key}: {group.Count()} logs");
        }
        
        // Alarm distribution
        var alarmsOverTime = allLogs.Where(l => l.HasNewAlarms).Count();
        Console.WriteLine();
        Console.WriteLine($"Alarm Distribution:");
        Console.WriteLine($"  Total Logs with Alarms: {alarmsOverTime}");
        Console.WriteLine($"  Alarm Rate:             {(alarmsOverTime * 100.0 / allLogs.Count):F2}%");

        Console.WriteLine("═══════════════════════════════════════════════════════════════");
    }
}

public class SimulatorConfig
{
    public string BrokerAddress { get; set; } = "localhost";
    public int BrokerPort { get; set; } = 1883;
    public int MachineCount { get; set; } = 80;
    public int LogsPerMinute { get; set; } = 500;
    public int DurationSeconds { get; set; } = 0; // 0 = continuous
    public bool BurstMode { get; set; } = false;
    public int BurstLogCount { get; set; } = 10000;
    public int BurstDurationSeconds { get; set; } = 30;
}

public class MachineState
{
    public string Token { get; set; } = string.Empty;
    public long Cycles { get; set; }
    public List<int> ActiveAlarms { get; set; } = new();
    public int? OperatorId { get; set; } = Random.Shared.Next(100, 200);
    public decimal Temperature { get; set; }
}

public class LogStatistic
{
    public DateTimeOffset Timestamp { get; set; }
    public string MachineToken { get; set; } = string.Empty;
    public bool Success { get; set; }
    public double DurationMs { get; set; }
    public bool HasNewAlarms { get; set; }
    public string? Error { get; set; }
}
