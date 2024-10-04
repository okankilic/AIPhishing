namespace AIPhishing.Database.Entities;

public class ClientTarget
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Department { get; set; }
    
    //  navigations
    public virtual Client Client { get; set; }
}