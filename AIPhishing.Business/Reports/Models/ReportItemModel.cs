namespace AIPhishing.Business.Reports.Models;

public record ReportItemModel(
    string Email,
    string Department,
    string ScenarioName,
    DateTime? SendDate,
    string Status);