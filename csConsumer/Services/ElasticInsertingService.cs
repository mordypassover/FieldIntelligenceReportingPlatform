using csConsumer.Models;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;

namespace csConsumer.Services;

public class ElasticsearchReportService
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticsearchReportService> _logger; 
    private const string IndexName = "reports";

    public ElasticsearchReportService(ElasticsearchClient client, ILogger<ElasticsearchReportService> logger)
    {
        _client = client;
        _logger = logger; 
    }

    public async Task CreateIndexAsync()
    {
        var existsResponse = await _client.Indices.ExistsAsync(IndexName);

        if (existsResponse.Exists)
        {
            _logger.LogInformation("Index '{IndexName}' already exists.", IndexName);
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

            _logger.LogInformation("Index '{IndexName}' created", IndexName);
        }
        else
        {
            _logger.LogError("Failed to create index '{IndexName}', details: {DebugInformation}",
                IndexName, response.DebugInformation);
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
            _logger.LogInformation("Indexed report successfully: {ReportId}", report.ReportId);
            return true;
        }

        _logger.LogError("Failed to index report: {ReportId}. Debug details: {DebugInformation}",
            report.ReportId, response.DebugInformation);

        return false;
    }
}