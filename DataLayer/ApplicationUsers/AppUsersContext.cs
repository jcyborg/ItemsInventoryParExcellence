using System;
using System.Collections.Generic;
using ItemsInventoryParExcellence.DataLayer.ApplicationUsers.Models;
using Microsoft.EntityFrameworkCore;

namespace ItemsInventoryParExcellence.DataLayer.ApplicationUsers;

public partial class AppUsersContext : DbContext
{
    public AppUsersContext()
    {
    }

    public AppUsersContext(DbContextOptions<AppUsersContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.ItemNo).HasName("PK__Items__727D9FE45C99D70F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
