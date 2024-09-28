namespace AIPhishing.Database.Entities;

public class Client
{
    public Guid Id { get; set; }
    public string ClientName { get; set; }
    public DateTime CreatedAt { get; set; }
    
    //  navigations
    public virtual ICollection<User> Users { get; set; }
    public virtual ICollection<ClientTarget> Targets { get; set; }
    // public virtual ICollection<Attack> Attacks { get; set; }
}