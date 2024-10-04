namespace AIPhishing.Business.Reports.Models;

public record ReportHeaderModel(
    long TotalPhishings, 
    int AveragePhishedEmployeesPercentage, 
    string MostPhishedDepartment,
    string BestDepartment);