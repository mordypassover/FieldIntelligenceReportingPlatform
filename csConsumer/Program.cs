using Confluent.Kafka;
using csConsumer.Models;
using csConsumer.Services;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging; 
using System.Text.Json;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("Appsettings.json",optional:true)
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();


services.AddLogging(builder =>
{
    builder.AddConsole(); 
});

services.AddSingleton<FilterIncommingReportService>();

var elasticSearchSettings = new ElasticsearchClientSettings(
    new Uri(configuration["ElasticSearch:Endpoint"])
);
var elasticSearchClient = new ElasticsearchClient(elasticSearchSettings);
services.AddSingleton(elasticSearchClient);
services.AddSingleton<ElasticsearchReportService>();

var serviceProvider = services.BuildServiceProvider();


var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

var validator = serviceProvider.GetRequiredService<FilterIncommingReportService>();

var elasticSearchService = serviceProvider.GetRequiredService<ElasticsearchReportService>();

await elasticSearchService.CreateIndexAsync();

var config = new ConsumerConfig
{
    BootstrapServers = configuration["Kafka:BootstrapServers"],
    GroupId = configuration["Kafka:GruopId"],
    AutoOffsetReset = AutoOffsetReset.Earliest
};

using var consumer = new ConsumerBuilder<string, string>(config).Build();
consumer.Subscribe(configuration["Kafka:Topics:Raw-data"]);


logger.LogInformation("Kafka consumer started");

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
            
            logger.LogWarning(ex, "Not valid JSON received: {Message}", ex.Message);
            continue;
        }

        if (report == null)
        {
            logger.LogWarning("Deserialized report = null");
            continue;
        }

        if (validator.Validate(report))
        {
            logger.LogInformation("Valid report: {ReportId}", report.ReportId);

            await elasticSearchService.IndexReportAsync(report);
        }
        else
        {
            logger.LogWarning("Invalid report didnt filter : {ReportId}", report.ReportId);
        }
    }
    catch (ConsumeException ex)
    {
        logger.LogError("Kafka consume error: {Reason}", ex.Error.Reason);
    }
}