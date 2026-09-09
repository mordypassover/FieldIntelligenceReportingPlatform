using Confluent.Kafka;
using csConsumer.Models;
using csConsumer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Appsettings.json")
    .Build();

var services = new ServiceCollection();

services.AddSingleton<FilterIncommingReportService>();

var serviceProvider = services.BuildServiceProvider();

var validator = serviceProvider
    .GetRequiredService<FilterIncommingReportService>();

var config = new ConsumerConfig
{
    BootstrapServers = configuration["Kafka:BootstrapServers"],
    GroupId = configuration["Kafka:GruopId"],
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using var consumer = new ConsumerBuilder<string, string>(config).Build();

consumer.Subscribe(configuration["Kafka:Topics:Raw-data"]);
var cnt = 0;
while (true)
{
    try
    {
        var consumeResult = consumer.Consume();

        string rawJsonReport = consumeResult.Message.Value;

        Report? report;

        try
        {
            report = JsonSerializer.Deserialize<Report>(rawJsonReport);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Invalid JSON: {ex.Message}");
            continue;
        }

        if (report == null)
        {
            Console.WriteLine("Report is null.");
            continue;
        }

        if (validator.Validate(report))
        {
            cnt ++;
            Console.WriteLine($"Valid report: {report.ReportId}");

            //add send to elastic!!!!
        }
        else
        {
            Console.WriteLine($"Invalid report: {report.ReportId}");
        }
    }
    catch (ConsumeException ex)
    {
        Console.WriteLine($"Kafka error: {ex.Error.Reason}");
    }
}