namespace AIPhishing.Business.Integrations.Models;

public record CrmGetUsersResponse(CrmUser[] Users);

public record CrmUser(string Email, string FullName);