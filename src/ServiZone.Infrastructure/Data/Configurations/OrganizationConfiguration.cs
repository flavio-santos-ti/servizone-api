using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiZone.Domain.Entities;

namespace ServiZone.Infrastructure.Data.Configurations;

/// <summary>
/// Configuração EF Core para a entidade Organization.
/// </summary>
public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
  public void Configure(EntityTypeBuilder<Organization> builder)
  {
    builder.ToTable("organizations");

    builder.HasKey(o => o.Id);

    builder.Property(o => o.Id)
        .HasColumnName("id")
        .IsRequired();

    builder.Property(o => o.Name)
        .HasColumnName("name")
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(o => o.Slug)
        .HasColumnName("slug")
        .HasMaxLength(100)
        .IsRequired();

    builder.HasIndex(o => o.Slug)
        .IsUnique()
        .HasDatabaseName("ix_organizations_slug");

    builder.Property(o => o.Status)
        .HasColumnName("status")
        .HasMaxLength(20)
        .IsRequired()
        .HasDefaultValue("active");

    builder.Property(o => o.Config)
        .HasColumnName("config")
        .HasColumnType("jsonb")
        .IsRequired()
        .HasDefaultValue("{}");

    builder.Property(o => o.CreatedAt)
        .HasColumnName("created_at")
        .HasColumnType("timestamptz")
        .IsRequired();

    builder.Property(o => o.UpdatedAt)
        .HasColumnName("updated_at")
        .HasColumnType("timestamptz")
        .IsRequired();
  }
}
