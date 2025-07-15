using System;
using System.Collections.Generic;

namespace EbayProject.Api.models;

public partial class ConnectionCountLog
{
    public int Id { get; set; }

    public string IpAddress { get; set; } = null!;

    public DateTime? ConnectionTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? ConnectionCount { get; set; }
}
