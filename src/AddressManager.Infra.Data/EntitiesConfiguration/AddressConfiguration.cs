using AddressManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AddressManager.Infra.Data.EntitiesConfiguration;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);

        builder.OwnsOne(a => a.ZipCode, zipCode =>
        {
            zipCode.Property(z => z.Value)
                .HasColumnName("ZipCode")
                .HasMaxLength(10)
                .IsRequired();
        });

        builder.OwnsOne(a => a.Street, street =>
        {
            street.Property(s => s.Value)
                .HasColumnName("Street")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.Property(a => a.Number)
            .HasColumnName("Number")
            .HasMaxLength(Address.MaxNumberLength);

        builder.Property(a => a.Complement)
            .HasColumnName("Complement")
            .HasMaxLength(Address.MaxComplementLength);

        builder.Property(a => a.Reference)
            .HasColumnName("Reference")
            .HasMaxLength(Address.MaxReferenceLength);

        builder.OwnsOne(a => a.Neighborhood, neighborhood =>
        {
            neighborhood.Property(n => n.Value)
                .HasColumnName("Neighborhood")
                .HasMaxLength(60)
                .IsRequired();
        });

        builder.OwnsOne(a => a.City, city =>
        {
            city.Property(c => c.Value)
                .HasColumnName("City")
                .HasMaxLength(30)
                .IsRequired();
        });

        builder.OwnsOne(a => a.State, state =>
        {
            state.Property(s => s.Abbreviation)
                .HasColumnName("StateAbbreviation")
                .HasMaxLength(2)
                .IsRequired();

            state.Property(s => s.Name)
                .HasColumnName("StateName")
                .HasMaxLength(25)
                .IsRequired();
        });

        builder.OwnsOne(a => a.Region, region =>
        {
            region.Property(r => r.Value)
                .HasColumnName("Region")
                .HasMaxLength(20)
                .IsRequired();
        });
    }
}
