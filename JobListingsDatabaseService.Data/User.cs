namespace JobListingsDatabaseService.Data;
public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid Interested { get; set; }

    public Guid Uninterested { get; set; }
}

