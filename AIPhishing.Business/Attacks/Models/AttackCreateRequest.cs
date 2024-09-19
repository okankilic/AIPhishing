using AIPhishing.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace AIPhishing.Business.Attacks.Models;

public record AttackCreateRequest(
    string Language, 
    AttackTypeEnum[]? Types, 
    IFormFile? CsvFile,
    string? From = null,
    string? DisplayName = null,
    string? Subject = null, 
    string? BodyTemplate = null,
    DateTime? StartTime = null);