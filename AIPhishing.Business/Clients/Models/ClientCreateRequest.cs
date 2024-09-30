using Microsoft.AspNetCore.Http;

namespace AIPhishing.Business.Clients.Models;

public record ClientCreateRequest(string ClientName, ClientUserEditModel User, IFormFile? CsvFile);