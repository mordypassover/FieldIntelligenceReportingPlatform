using System.ComponentModel.DataAnnotations;
using csConsumer.Models;

namespace csConsumer.Services;

internal class FilterIncommingReportService
{
    private readonly List<string> _usedIds = new();

    public bool Validate(Report report)
    {
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
                Console.WriteLine($"Validation error: {result.ErrorMessage}");
            }

            return false;
        }

        if (string.IsNullOrWhiteSpace(report.ReportId)|| string.IsNullOrWhiteSpace(report.Unit)||
            string.IsNullOrWhiteSpace(report.AgentId) || string.IsNullOrWhiteSpace(report.Theater) ||
            string.IsNullOrWhiteSpace(report.Sector) || string.IsNullOrWhiteSpace(report.Location) ||
            string.IsNullOrWhiteSpace(report.ReportType) || string.IsNullOrWhiteSpace(report.Priority) ||
            string.IsNullOrWhiteSpace(report.SourceType) || string.IsNullOrWhiteSpace(report.Message))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(report.SubjectId) !=
            string.IsNullOrWhiteSpace(report.SubjectType))
        {
            return false;
        }

        if (_usedIds.Contains(report.ReportId))
        {
            Console.WriteLine($"Duplicate ReportId: {report.ReportId}");
            return false;
        }

        _usedIds.Add(report.ReportId);

        return true;
    }
}