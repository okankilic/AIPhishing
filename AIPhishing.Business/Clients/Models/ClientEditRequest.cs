using Microsoft.AspNetCore.Http;

namespace AIPhishing.Business.Clients.Models;

public record ClientEditRequest(string ClientName, ClientUserEditModel User, IFormFile? CsvFile);