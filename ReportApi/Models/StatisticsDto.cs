namespace ReportApi.Models;

public class StatisticsDto
{
    
    public Dictionary<string, long> ReportsByPriority { get; set; } = new();
    public Dictionary<string, long> ReportsByTheater { get; set; } = new();
    public Dictionary<string, long> ReportsByReportType { get; set; } = new();
    
}
