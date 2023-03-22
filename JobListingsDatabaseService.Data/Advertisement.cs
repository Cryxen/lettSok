namespace JobListingsDatabaseService.Data;
public class Advertisement
{
   

    public string? Uuid
    {
        get;
        set;
    }
    public DateTime Expires
    {
        get;
        set;
    }
    public string? Municipal
    {
        get;
        set;
    }
    public string? Title
    {
        get;
        set;
    }
    public string? Description
    {
        get;
        set;
    }
    public string? JobTitle
    {
        get;
        set;
    }
    public string? Employer
    {
        get;
        set;
    }
    public string? EngagementType
    {
        get;
        set;
    }

    /*
     * Virtual er funnet fra:
     *	https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/workflows/new-database  
     */
    //public virtual List<Advertisement>? Advertisements { get; set; }

    /*
     * Many to many:
     * https://www.entityframeworktutorial.net/code-first/configure-many-to-many-relationship-in-code-first.aspx?utm_content=cmp-true
     
    public virtual ICollection<User>? Users { get; set; }
    */
    public ICollection<InterestedAdvertisement> interestedAdvertisements { get; set; }
    public ICollection<UninterestedAdvertisement> uninterestedAdvertisements { get; set; }

}

