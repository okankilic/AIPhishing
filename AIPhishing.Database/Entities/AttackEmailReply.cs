﻿namespace AIPhishing.Database.Entities;

public class AttackEmailReply
{
    public Guid Id { get; set; }
    public Guid AttackEmailId { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; }
    
    //  navigations
    public virtual AttackEmail AttackEmail { get; set; }
}