using Microsoft.AspNetCore.Mvc;
using ReportApi.Models;
using ReportApi.repositorys;
using Microsoft.Extensions.Logging; // [ADDED] Introduced logging namespace

namespace ReportApi.Controllers;

[ApiController]
[Route("api/reports")]
public class ElasticController : ControllerBase
{
    private readonly IElasticRepository _elasticRepository;
    private readonly ILogger<ElasticController> _logger; 

    
    public ElasticController(IElasticRepository elasticRepository, ILogger<ElasticController> logger)
    {
        _elasticRepository = elasticRepository;
        _logger = logger; 
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Report>>> GetReportsAsync([FromQuery] string text)
    {
    
        _logger.LogInformation("HTTP GET api/reports/search requested for text: '{Text}'", text);

        var result = await _elasticRepository.SearchMesegesAsync(text);
        return Ok(result);
    }

    [HttpGet("/api/subjects/{subjectId}/reports")]
    public async Task<ActionResult<IEnumerable<Report>>> GetSubjectReportsAsync(string subjectId)
    {
        
        _logger.LogInformation("HTTP GET /api/subjects/{SubjectId}/reports requested", subjectId);

        var result = await _elasticRepository.SearchSubjectReportsAsync(subjectId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Report>>> FilterByTheaterSectorOrLocationAsync(
        [FromQuery] string? theater,
        [FromQuery] string? sector,
        [FromQuery] string? location)
    {
  
        _logger.LogInformation("HTTP GET api/reports requested - Theater: '{Theater}', Sector: '{Sector}', Location: '{Location}'",
            theater, sector, location);

        var result = await _elasticRepository.FilterTheaterSectorOrLocation(theater, sector, location);
        return Ok(result);
    }

    [HttpGet("by-priority")]
    public async Task<ActionResult<IEnumerable<Report>>> GetByPriorityAndDateAsync(
        [FromQuery] List<string>? priorities,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
      
        _logger.LogInformation("HTTP GET api/reports/by-priority requested - Priorities: {Priorities}, From: {From}, To: {To}",
            priorities != null ? string.Join(", ", priorities) : "None", from, to);

        var result = await _elasticRepository.FilterByPriorityAndDateAsync(priorities, from, to);
        return Ok(result);
    }

    [HttpGet("search/advanced")]
    public async Task<ActionResult<IEnumerable<Report>>> SearchReportsAsync(
        [FromQuery] string? search,
        [FromQuery] string? theater,
        [FromQuery] string? sector,
        [FromQuery] string? location,
        [FromQuery] string? priority,
        [FromQuery] string? reportType,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
      
        _logger.LogInformation("HTTP GET api/reports/search/advanced requested - Search: '{Search}', Theater: '{Theater}', Sector: '{Sector}', Location: '{Location}', Priority: '{Priority}', Type: '{ReportType}'",
            search, theater, sector, location, priority, reportType);

        var result = await _elasticRepository.SearchReportsAsync(search, theater, sector, location, priority, reportType, from, to);
        return Ok(result);
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<StatisticsDto>> GetStatistics()
    {

        _logger.LogInformation("HTTP GET api/reports/statistics requested");

        var result = await _elasticRepository.GetStatisticsAsync();
        return Ok(result);
    }
}