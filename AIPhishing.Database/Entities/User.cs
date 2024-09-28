namespace AIPhishing.Database.Entities;

public class User
{
    public Guid Id { get; set; }
    public Guid? ClientId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    //  navigations
    public virtual Client Client { get; set; }
}