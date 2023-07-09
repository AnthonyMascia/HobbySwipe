using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HobbySwipe.Data.Entities;

public partial class HobbySwipeContext : DbContext
{
    public HobbySwipeContext()
    {
    }

    public HobbySwipeContext(DbContextOptions<HobbySwipeContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionsOption> QuestionsOptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:hobbyswipe-test-eastus.database.windows.net,1433;Initial Catalog=HobbySwipe;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication=Active Directory Interactive;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.QuestionId)
                .HasMaxLength(15)
                .HasColumnName("QuestionID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answers__Questio__17F790F9");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(e => e.Id)
                .HasMaxLength(15)
                .HasColumnName("ID");
            entity.Property(e => e.NextQuestionId).HasMaxLength(15);
        });

        modelBuilder.Entity<QuestionsOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Questions_Options");

            entity.ToTable("Questions.Options");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.NextQuestionId).HasMaxLength(15);
            entity.Property(e => e.QuestionId)
                .HasMaxLength(15)
                .HasColumnName("QuestionID");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionsOptions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Questions__Quest__1AD3FDA4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
