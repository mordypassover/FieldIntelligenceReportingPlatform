using ReportApi.Models;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging; 
namespace ReportApi.repositorys;

public class ElasticRepository : IElasticRepository
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticRepository> _logger; 

    public ElasticRepository(ElasticsearchClient client, ILogger<ElasticRepository> logger)
    {
        _client = client;
        _logger = logger; 
    }

    public async Task<IEnumerable<Report>> SearchMesegesAsync(string search)
    {
      
        _logger.LogInformation(" Search for term: '{SearchTerm}'", search);

        var response = await _client.SearchAsync<Report>(s => s
            .Index("reports")
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Message)
                    .Query(search)
                )
            )
        );

        if (!response.IsValidResponse)
        {
            
            _logger.LogError("Failed to SearchMesegesAsync. Debug details: {DebugInformation}", response.DebugInformation);
        }

        return response.Documents;
    }

    public async Task<IEnumerable<Report>> SearchSubjectReportsAsync(string subjectId)
    {
       
        _logger.LogInformation("Searching reports for SubjectId: '{SubjectId}'", subjectId);

        var response = await _client.SearchAsync<Report>(s => s
            .Index("reports")
            .Query(q => q
                .Term(t => t
                    .Field(f => f.SubjectId)
                    .Value(subjectId)
                )
            )
        );

        if (!response.IsValidResponse)
        {
           
            _logger.LogError("SubjectId '{SubjectId}' not found. Debug details: {DebugInformation}",
                subjectId, response.DebugInformation);
            throw new Exception(response.DebugInformation);
        }

        return response.Documents;
    }

    public async Task<IEnumerable<Report>> FilterTheaterSectorOrLocation(
        string? theater,
        string? sector,
        string? location)
    {
        
        _logger.LogInformation("Filtering reports Theater: '{Theater}', Sector: '{Sector}', Location: '{Location}'",
            theater, sector, location);

        var filters = new List<Query>();

        if (!string.IsNullOrWhiteSpace(theater))
        {
            filters.Add(
                new TermQuery(Infer.Field<Report>(r => r.Theater))
                {
                    Value = theater
                }
            );
        }

        if (!string.IsNullOrWhiteSpace(sector))
        {
            filters.Add(
                new TermQuery(Infer.Field<Report>(r => r.Sector))
                {
                    Value = sector
                }
            );
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            filters.Add(
                new TermQuery(Infer.Field<Report>(r => r.Location))
                {
                    Value = location
                }
            );
        }

        var response = await _client.SearchAsync<Report>(s => s
            .Index("reports")
            .Query(q => q
                .Bool(b => b
                    .Filter(filters)
                )
            )
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError("Failed to filter by theater/sector/location. Debug details: {DebugInformation}", response.DebugInformation);
        }

        return response.Documents;
    }

    public async Task<IEnumerable<Report>> FilterByPriorityAndDateAsync(
        List<string>? priorities,
        DateTime? from,
        DateTime? to)
    {
        _logger.LogInformation("Filtering reports - Priorities: {Priorities}, From: {From}, To: {To}",
            priorities != null ? string.Join(", ", priorities) : "None", from, to);

        var filters = new List<Query>();

        if (priorities != null && priorities.Any())
        {
            var termsValues = priorities.Select(p => (FieldValue)p).ToArray();

            filters.Add(new TermsQuery
            {
                Field = Infer.Field<Report>(f => f.Priority),
                Term = new TermsQueryField(termsValues)
            });
        }
        if (from.HasValue || to.HasValue)
        {
            filters.Add(new DateRangeQuery(Infer.Field<Report>(f => f.Timestamp))
            {
                Gte = from,
                Lte = to
            });
        }

        var response = await _client.SearchAsync<Report>(s => s
            .Index("reports")
            .Query(q => q
                .Bool(b => b
                    .Filter(filters.ToArray())
                )
            )
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError("Failed to filter by priority and date. Debug details: {DebugInformation}", response.DebugInformation);
        }

        return response.Documents;
    }

    public async Task<IEnumerable<Report>> SearchReportsAsync(
        string? search,
        string? theater,
        string? sector,
        string? location,
        string? priorety,
        string? reportType,
        DateTime? from,
        DateTime? to)
    {
        _logger.LogInformation("Running multi-field search - Search: '{Search}',Theater: '{Theater}', Sector: '{Sector}', Location: '{Location}', Priority: '{Priority}', Type: '{ReportType}'",
            search, theater, sector, location, priorety, reportType);

        var filters = new List<Query>();

        if (!string.IsNullOrEmpty(theater))
        {
            filters.Add(
                new TermQuery(Infer.Field<Report>(r => r.Theater))
                {
                    Value = theater
                }
            );
        }

        if (!string.IsNullOrEmpty(sector))
        {
            filters.Add(
                new TermQuery(Infer.Field<Report>(r => r.Sector))
                {
                    Value = sector
                }
            );
        }

        if (!string.IsNullOrEmpty(location))
        {
            filters.Add(
                new TermQuery(Infer.Field<Report>(r => r.Location))
                {
                    Value = location
                }
            );
        }

        if (!string.IsNullOrEmpty(priorety))
        {
            filters.Add(
                new TermQuery(Infer.Field<Report>(r => r.Priority))
                {
                    Value = priorety
                }
            );
        }

        if (!string.IsNullOrEmpty(reportType))
        {
            filters.Add(
                new TermQuery(Infer.Field<Report>(r => r.ReportType))
                {
                    Value = reportType
                }
            );
        }

        if (from.HasValue || to.HasValue)
        {
            filters.Add(
                new DateRangeQuery(Infer.Field<Report>(r => r.Timestamp))
                {
                    Gte = from,
                    Lte = to
                }
            );
        }

        var response = await _client.SearchAsync<Report>(s => s
            .Index("reports")
            .Query(q => q
                .Bool(b =>
                {
                    var mustQueries = new List<Query>();

                    if (!string.IsNullOrEmpty(search))
                    {
                        mustQueries.Add(
                            new MatchQuery(Infer.Field<Report>(r => r.Message))
                            {
                                Query = search
                            }
                        );
                    }

                    var boolQuery = new BoolQuery
                    {
                        Must = mustQueries,
                        Filter = filters
                    };
                })
            )
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError("Failed executing SearchReportsAsync.Debug details: {DebugInformation}", response.DebugInformation);
        }

        return response.Documents;
    }

    public async Task<StatisticsDto> GetStatisticsAsync()
    {
        _logger.LogInformation("Getting report statistics from Elasticsearch.");

        var response = await _client.SearchAsync<Report>(s => s
            .Index("reports")
            .Size(0)
            .Aggregations(a => a
                .Add("by_priority", ag => ag.Terms(t => t.Field(f => f.Priority)))
                .Add("by_theater", ag => ag.Terms(t => t.Field(f => f.Theater)))
                .Add("by_report_type", ag => ag.Terms(t => t.Field(f => f.ReportType)))
            )
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError("Failed to fetch statistics . Debug details: {DebugInformation}", response.DebugInformation);
        }

        var result = new StatisticsDto();

        if (response.Aggregations != null)
        {
            if (response.Aggregations.TryGetValue("by_priority", out var priorityAggregate) && priorityAggregate is StringTermsAggregate priorityTerms)
            {
                result.ReportsByPriority = priorityTerms.Buckets.ToDictionary(b => b.Key.ToString(), b => b.DocCount);
            }

            if (response.Aggregations.TryGetValue("by_theater", out var theaterAggregate) && theaterAggregate is StringTermsAggregate theaterTerms)
            {
                result.ReportsByTheater = theaterTerms.Buckets.ToDictionary(b => b.Key.ToString(), b => b.DocCount);
            }

            if (response.Aggregations.TryGetValue("by_report_type", out var typeAggregate) && typeAggregate is StringTermsAggregate typeTerms)
            {
                result.ReportsByReportType = typeTerms.Buckets.ToDictionary(b => b.Key.ToString(), b => b.DocCount);
            }
        }
        _logger.LogInformation("Successfully retrieved statistics");

        return result;
    }
}