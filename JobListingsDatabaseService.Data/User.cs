namespace JobListingsDatabaseService.Data;
public class User
{
  

    public Guid Id { get; set; }

    public string Name { get; set; }


    //public virtual ICollection<Advertisement> Uninterested { get; set; }

    /*
 * Many to many:
 * https://www.entityframeworktutorial.net/code-first/configure-many-to-many-relationship-in-code-first.aspx?utm_content=cmp-true
 
    public virtual ICollection<Advertisement>? Advertisements { get; set; }
*/
    public ICollection<UninterestedAdvertisement> uninterestedAdvertisements { get; set; }
    public ICollection<InterestedAdvertisement> interestedAdvertisements { get; set; }

}




