using UserPreferencesDatabaseService.Data;

namespace UserPreferencesDatabaseService.Data;
public class User
{
  

    public Guid Id { get; set; }

    public string Name { get; set; }

    public ICollection<InterestedAdvertisement> InterestedAdvertisements { get; set; }
    public ICollection<UninterestedAdvertisement> UninterestedAdvertisements { get; set; }
    public ICollection<SearchLocation> SearchLocations { get; set; }

}




