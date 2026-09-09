namespace KMC.Web.Models;

public class HomeViewModel
{
    public List<EventViewModel> FeaturedEvents { get; set; } = new();
    public int TotalEvents { get; set; }
    public int TotalParticipants { get; set; }
    public int TotalOrganizers { get; set; }
    public int TotalCategories { get; set; }
}
