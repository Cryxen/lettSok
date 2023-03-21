namespace UserPreferencesDatabaseService.Data;
public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public List<string> Interested { get; set; }

    public List<string> Uninterested { get; set; }
}

