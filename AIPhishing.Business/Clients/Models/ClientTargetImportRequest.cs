using Microsoft.AspNetCore.Http;

namespace AIPhishing.Business.Clients.Models;

public record ClientTargetImportRequest(IFormFile File);