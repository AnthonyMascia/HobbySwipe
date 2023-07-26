using System;
using System.Collections.Generic;

namespace HobbySwipe.Data.Entities;

public partial class Hobby
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public virtual ICollection<HobbiesSynonym> HobbiesSynonyms { get; set; } = new List<HobbiesSynonym>();
}
