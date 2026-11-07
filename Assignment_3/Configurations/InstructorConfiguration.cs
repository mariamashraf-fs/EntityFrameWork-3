using Assignment_3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;


namespace Assignment_3.Configurations
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.HasKey(i => i.InstructorId);

            builder.Property(i => i.FullName)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(i => i.Email)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(i => i.Bio)
                   .HasMaxLength(500);

            builder.Property(i => i.JoinDate)
                   .IsRequired();
        }
    }
}
