using System;
using System.Collections.Generic;

namespace HobbySwipe.Data.Entities;

public partial class HobbiesSynonym
{
    public int Id { get; set; }

    public string HobbyId { get; set; }

    public string Synonym { get; set; }

    public virtual Hobby Hobby { get; set; }
}
