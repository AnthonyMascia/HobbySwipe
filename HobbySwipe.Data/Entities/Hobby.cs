namespace HobbySwipe.Data.Entities;

public partial class Hobby
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Slug { get; set; }

    public string Description { get; set; }

    public virtual ICollection<HobbiesSynonym> HobbiesSynonyms { get; set; } = new List<HobbiesSynonym>();
}
