using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace csConsumer.Models;

internal class Report
{
    [Required]

    [JsonPropertyName("reportId")]
    public string ReportId { get; set; }

    [Required]
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
    [Required]
    [JsonPropertyName("agentId")]
    public string AgentId { get; set; }
    [Required]
    [JsonPropertyName("unit")]
    public string Unit { get; set; }
    [Required]
    [JsonPropertyName("theater")]
    public string Theater { get; set; }
    [Required]
    [JsonPropertyName("sector")]
    public string Sector { get; set; }
    [Required]
    [JsonPropertyName("location")]
    public string Location { get; set; }
    [Required]
    [JsonPropertyName("reportType")]
    [RegularExpression(
        "^(Observation|Movement|Meeting|Access|Communication|Logistics|Incident)$")]

    public string ReportType { get; set; }
    [Required]
    [JsonPropertyName("priority")]
    [RegularExpression("^(Low|Medium|High|Critical)$")]
    public string Priority { get; set; }
    [Required]
    [JsonPropertyName("sourceType")]
    public string SourceType { get; set; }
    [Required]
    [JsonPropertyName("message")]
    public string Message { get; set; }
    [JsonPropertyName("subjectId")]
    public string? SubjectId { get; set; }
    [JsonPropertyName("subjectType")]
    public string? SubjectType { get; set; }
}
