using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HobbySwipe.Data.Entities.HobbySwipe;

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

    public virtual DbSet<HobbiesSynonym> HobbiesSynonyms { get; set; }

    public virtual DbSet<Hobby> Hobbies { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionsOption> QuestionsOptions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:hobbyswipe-test-eastus.database.windows.net,1433;Initial Catalog=HobbySwipe;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication=Active Directory Interactive;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.Property(e => e.QuestionId)
                .IsRequired()
                .HasMaxLength(15)
                .HasColumnName("QuestionID");
            entity.Property(e => e.Response).IsRequired();
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Question).WithMany(p => p.Answers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Answers__Questio__3587F3E0");
        });

        modelBuilder.Entity<HobbiesSynonym>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Hobbies.__3214EC07B5E8FC15");

            entity.ToTable("Hobbies.Synonyms");

            entity.Property(e => e.HobbyId)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Synonym)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.Hobby).WithMany(p => p.HobbiesSynonyms)
                .HasForeignKey(d => d.HobbyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Hobbies.S__Hobby__5F7E2DAC");
        });

        modelBuilder.Entity<Hobby>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Hobbies__3214EC07723417E8");

            entity.Property(e => e.Id).HasMaxLength(100);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.Property(e => e.Id).HasMaxLength(15);
            entity.Property(e => e.NextQuestionId).HasMaxLength(15);
            entity.Property(e => e.QuestionText).IsRequired();
        });

        modelBuilder.Entity<QuestionsOption>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Questions_Options");

            entity.ToTable("Questions.Options");

            entity.Property(e => e.NextQuestionId).HasMaxLength(15);
            entity.Property(e => e.OptionText).IsRequired();
            entity.Property(e => e.QuestionId)
                .IsRequired()
                .HasMaxLength(15)
                .HasColumnName("QuestionID");

            entity.HasOne(d => d.Question).WithMany(p => p.QuestionsOptions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Questions__Quest__3864608B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
