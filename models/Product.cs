using System;
using System.Collections.Generic;

namespace EbayProject.Api.models;

public partial class Product
{
    public int Id { get; set; }

    public int SellerId { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool? Deleted { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Listing> Listings { get; set; } = new List<Listing>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual User Seller { get; set; } = null!;
}
