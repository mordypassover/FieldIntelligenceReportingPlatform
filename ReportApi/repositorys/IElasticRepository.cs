using ReportApi.Models;
using System.Collections.Generic;


namespace ReportApi.repositorys;

public interface IElasticRepository
{
    Task<IEnumerable<Report>> SearchMesegesAsync(string search);
    Task<IEnumerable<Report>> SearchSubjectReportsAsync(string subjectId);
    Task<IEnumerable<Report>> FilterTheaterSectorOrLocation(
        string? theater,
        string? sector,
        string? location);
    Task<IEnumerable<Report>> FilterByPriorityAndDateAsync(
        List<string>? priorities,
        DateTime? from,
        DateTime? to);
    Task<IEnumerable<Report>> SearchReportsAsync(
    string? search,
    string? theater,
    string? sector,
    string? location,
    string? urgency,
    string? reportType,
    DateTime? from,
    DateTime? to);
    Task<StatisticsDto> GetStatisticsAsync();
}