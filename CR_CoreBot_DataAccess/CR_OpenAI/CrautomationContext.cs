using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CR_CoreBot_DataAccess.CR_OpenAI;

public partial class CrautomationContext : DbContext
{
    public CrautomationContext()
    {
    }

    public CrautomationContext(DbContextOptions<CrautomationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EntityExtractrion> EntityExtractrions { get; set; }

    public virtual DbSet<TblCustomerFileDetail> TblCustomerFileDetails { get; set; }

    public virtual DbSet<TblExceptionDetail> TblExceptionDetails { get; set; }

    public virtual DbSet<TblSummaryExtractrion> TblSummaryExtractrions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
  => optionsBuilder.UseSqlServer("Server=DESKTOP-ROP0QHV;Database=CROpenAI;Encrypt=False;Integrated Security=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EntityExtractrion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EntityEx__3214EC07F4D10CD4");

            entity.ToTable("EntityExtractrion");

            entity.Property(e => e.Entity)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FileName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.ImportantNotes)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblCustomerFileDetail>(entity =>
        {
            entity.ToTable("Tbl_CustomerFileDetails");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FileName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FilePath).IsUnicode(false);
        });

        modelBuilder.Entity<TblExceptionDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_LoggerDetail");

            entity.ToTable("Tbl_ExceptionDetails");

            entity.Property(e => e.ActionName).HasMaxLength(500);
            entity.Property(e => e.ControllerName).HasMaxLength(500);
            entity.Property(e => e.ExceptionMessage).HasMaxLength(500);
            entity.Property(e => e.ExceptionStakeTrace).HasMaxLength(500);
            entity.Property(e => e.LogTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<TblSummaryExtractrion>(entity =>
        {
            entity.ToTable("tbl_SummaryExtractrion");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.FileName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.SummaryDescription).IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
