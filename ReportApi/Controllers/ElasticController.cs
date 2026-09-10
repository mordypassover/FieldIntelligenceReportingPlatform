using Microsoft.AspNetCore.Mvc;
using ReportApi.Models;
using ReportApi.repositorys;

namespace ReportApi.Controllers;

[ApiController]
[Route("api/reports")]
public class ElasticController : ControllerBase
{
    private readonly IElasticRepository _elasticRepository;

    public ElasticController(IElasticRepository elasticRepository)
    {
        _elasticRepository = elasticRepository;
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Report>>> GetReportsAsync([FromQuery] string text)
    {
        return Ok(await _elasticRepository.SearchMesegesAsync(text));
    }

    [HttpGet("/api/subjects/{subjectId}/reports")]
    public async Task<ActionResult<IEnumerable<Report>>> GetSubjectReportsAsync(string subjectId)
    {
        return Ok(await _elasticRepository.SearchSubjectReportsAsync(subjectId));
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Report>>> FilterByTheaterSectorOrLocationAsync(
        [FromQuery] string? theater,
        [FromQuery] string? sector,
        [FromQuery] string? location)
    {
        return Ok(await _elasticRepository.FilterTheaterSectorOrLocation(theater, sector, location));
    }


    [HttpGet("by-priority")]
    public async Task<ActionResult<IEnumerable<Report>>> GetByPriorityAndDateAsync(
        [FromQuery] List<string>? priorities,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        return Ok(await _elasticRepository.FilterByPriorityAndDateAsync(priorities, from, to));
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
        return Ok(await _elasticRepository.SearchReportsAsync(search, theater, sector, location, priority, reportType, from, to));
    }


    [HttpGet("statistics")]
    public async Task<ActionResult<StatisticsDto>> GetStatistics()
    {
        return Ok(await _elasticRepository.GetStatisticsAsync());
    }
}