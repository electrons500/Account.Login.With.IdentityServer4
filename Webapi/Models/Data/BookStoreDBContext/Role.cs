using System;
using System.Collections.Generic;

namespace Webapi.Models.Data.BookStoreDBContext;

public partial class Role
{
    public int RoleId { get; set; }

    public string? Name { get; set; }

    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
