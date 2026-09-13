using System.ComponentModel.DataAnnotations;
using csConsumer.Models;
using Microsoft.Extensions.Logging;

namespace csConsumer.Services;

internal class FilterIncommingReportService
{
    private readonly List<string> _usedIds = new();
    private readonly ILogger<FilterIncommingReportService> _logger;

    public FilterIncommingReportService(ILogger<FilterIncommingReportService> logger)
    {
        _logger = logger;
    }

    public bool Validate(Report report)
    {
        _logger.LogInformation("Start validation for report : {ReportId}", report?.ReportId);

        var context = new ValidationContext(report);
        var validationResults = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(
            report,
            context,
            validationResults,
            validateAllProperties: true
        );

        if (!isValid)
        {
            foreach (var result in validationResults)
            {
                _logger.LogWarning("Validation error: {ErrorMessage}", result.ErrorMessage);
            }

            return false;
        }

        if (string.IsNullOrWhiteSpace(report.ReportId) || string.IsNullOrWhiteSpace(report.Unit) ||
            string.IsNullOrWhiteSpace(report.AgentId) || string.IsNullOrWhiteSpace(report.Theater) ||
            string.IsNullOrWhiteSpace(report.Sector) || string.IsNullOrWhiteSpace(report.Location) ||
            string.IsNullOrWhiteSpace(report.ReportType) || string.IsNullOrWhiteSpace(report.Priority) ||
            string.IsNullOrWhiteSpace(report.SourceType) || string.IsNullOrWhiteSpace(report.Message))
        {
            _logger.LogWarning("Validation failed report hase empty or whitespace fialdes");
            return false;
        }

        if (string.IsNullOrWhiteSpace(report.SubjectId) != string.IsNullOrWhiteSpace(report.SubjectType))
        {
            _logger.LogWarning("Validation failed SubjectId and SubjectType are invalid");
            return false;
        }

        if (_usedIds.Contains(report.ReportId))
        {
            _logger.LogWarning("ReportId in use detected: {ReportId}", report.ReportId);
            return false;
        }

        _usedIds.Add(report.ReportId);
        _logger.LogInformation("Report {ReportId} successfully validated.", report.ReportId);

        return true;
    }
}