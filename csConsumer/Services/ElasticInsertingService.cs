using csConsumer.Models;
using Elastic.Clients.Elasticsearch;

namespace csConsumer.Services;

public class ElasticsearchReportService
{
    private readonly ElasticsearchClient _client;
    private const string IndexName = "reports";

    public ElasticsearchReportService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task CreateIndexAsync()
    {
        var existsResponse = await _client.Indices.ExistsAsync(IndexName);

        if (existsResponse.Exists)
        {
            Console.WriteLine($"Index '{IndexName}' already exists.");
            return;
        }

        var response = await _client.Indices.CreateAsync(IndexName, c => c
            .Mappings(m => m
                .Properties<Report>(p => p
                    .Keyword(x => x.ReportId)
                    .Date(x => x.Timestamp)
                    .Keyword(x => x.AgentId)
                    .Keyword(x => x.Unit)
                    .Keyword(x => x.Theater)
                    .Keyword(x => x.Sector)
                    .Keyword(x => x.Location)
                    .Keyword(x => x.ReportType)
                    .Keyword(x => x.Priority)
                    .Keyword(x => x.SourceType)
                    .Text(x => x.Message)
                    .Keyword(x => x.SubjectId)
                    .Keyword(x => x.SubjectType)
                )
            )
        );

        if (response.IsValidResponse)
        {
            Console.WriteLine($"Index '{IndexName}' created successfully.");
        }
        else
        {
            Console.WriteLine($"Failed to create index '{IndexName}'.");
            Console.WriteLine(response.DebugInformation);
        }
    }

    public async Task<bool> IndexReportAsync(Report report)
    {
        var response = await _client.IndexAsync(
            report,
            i => i
                .Index(IndexName)
                .Id(report.ReportId)
        );

        if (response.IsValidResponse)
        {
            Console.WriteLine($"Indexed successfully: {report.ReportId}");
            return true;
        }

        Console.WriteLine($"Failed to index report: {report.ReportId}");
        Console.WriteLine(response.DebugInformation);

        return false;
    }
}

