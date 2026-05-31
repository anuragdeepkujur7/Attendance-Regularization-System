using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Project_6_final.Models;

public partial class AttendanceContext : DbContext
{
    public AttendanceContext()
    {
    }

    public AttendanceContext(DbContextOptions<AttendanceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Education> Educations { get; set; }

    public virtual DbSet<Invalidtoken> Invalidtokens { get; set; }

    public virtual DbSet<Monthlyattendance> Monthlyattendances { get; set; }

    public virtual DbSet<Regularizationrequest> Regularizationrequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=attendance;username=root;password=root", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.39-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PRIMARY");

            entity.ToTable("address");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.Street).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("address_ibfk_1");
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.AttendanceId).HasName("PRIMARY");

            entity.ToTable("attendance");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.CreatedOn)
                .HasColumnType("timestamp")
                .HasColumnName("CreatedON");
            entity.Property(e => e.LoginTime).HasColumnType("timestamp");
            entity.Property(e => e.LogoutTime).HasColumnType("timestamp");
            entity.Property(e => e.Status).HasColumnType("enum('Present','Absent','Regularized')");
            entity.Property(e => e.UpdatedOn).HasColumnType("timestamp");

            entity.HasOne(d => d.User).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("attendance_ibfk_1");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PRIMARY");

            entity.ToTable("contacts");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.Phone).HasMaxLength(15);

            entity.HasOne(d => d.User).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("contacts_ibfk_1");
        });

        modelBuilder.Entity<Education>(entity =>
        {
            entity.HasKey(e => e.EducationId).HasName("PRIMARY");

            entity.ToTable("education");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.Degree).HasMaxLength(45);
            entity.Property(e => e.FieldOfStudy).HasMaxLength(45);
            entity.Property(e => e.Institution).HasMaxLength(100);
            entity.Property(e => e.Percentage).HasPrecision(5, 2);

            entity.HasOne(d => d.User).WithMany(p => p.Educations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("education_ibfk_1");
        });

        modelBuilder.Entity<Invalidtoken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("invalidtokens");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.Expiration).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.Invalidtokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("invalidtokens_ibfk_1");
        });

        modelBuilder.Entity<Monthlyattendance>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PRIMARY");

            entity.ToTable("monthlyattendance");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.GeneratedAt).HasColumnType("timestamp");

            entity.HasOne(d => d.User).WithMany(p => p.Monthlyattendances)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("monthlyattendance_ibfk_1");
        });

        modelBuilder.Entity<Regularizationrequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PRIMARY");

            entity.ToTable("regularizationrequest");

            entity.HasIndex(e => e.AttendanceId, "AttendanceId");

            entity.HasIndex(e => e.RequestedBy, "FK_RequestedBy");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.CreatedOn).HasColumnType("timestamp");
            entity.Property(e => e.Reason).HasColumnType("text");
            entity.Property(e => e.Status).HasColumnType("enum('Approved','Denied')");
            entity.Property(e => e.UpdatedOn).HasColumnType("timestamp");

            entity.HasOne(d => d.Attendance).WithMany(p => p.Regularizationrequests)
                .HasForeignKey(d => d.AttendanceId)
                .HasConstraintName("regularizationrequest_ibfk_2");

            entity.HasOne(d => d.RequestedByNavigation).WithMany(p => p.RegularizationrequestRequestedByNavigations)
                .HasForeignKey(d => d.RequestedBy)
                .HasConstraintName("FK_RequestedBy");

            entity.HasOne(d => d.User).WithMany(p => p.RegularizationrequestUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("regularizationrequest_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasColumnType("enum('Male','Female','Others')");
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Roles)
                .HasColumnType("enum('Admin','User')")
                .HasColumnName("roles");
            entity.Property(e => e.UpdatedOn)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
