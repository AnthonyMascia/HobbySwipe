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

    public virtual DbSet<Action> Actions { get; set; }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<Attribute> Attributes { get; set; }

    public virtual DbSet<CategoriesHobbiesAttribute> CategoriesHobbiesAttributes { get; set; }

    public virtual DbSet<CategoriesHobbiesSynonym> CategoriesHobbiesSynonyms { get; set; }

    public virtual DbSet<CategoriesHobby> CategoriesHobbies { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionsOption> QuestionsOptions { get; set; }

    public virtual DbSet<UserHobbyPreference> UserHobbyPreferences { get; set; }

    public virtual DbSet<UserHobbyPreferencesHistory> UserHobbyPreferencesHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:hobbyswipe-test-eastus.database.windows.net,1433;Initial Catalog=HobbySwipe;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Authentication=Active Directory Interactive;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Action>(entity =>
        {
            entity.HasKey(e => e.ActionId).HasName("PK__Actions__FFE3F4D9C33A1872");

            entity.Property(e => e.ActionId).ValueGeneratedNever();
            entity.Property(e => e.ActionName)
                .IsRequired()
                .HasMaxLength(50);
        });

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

        modelBuilder.Entity<Attribute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attribut__3214EC076EA971A0");

            entity.Property(e => e.Id).HasMaxLength(100);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<CategoriesHobbiesAttribute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07EABB49C4");

            entity.ToTable("Categories.Hobbies.Attributes");

            entity.Property(e => e.AttributeId)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.HobbyId)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.Attribute).WithMany(p => p.CategoriesHobbiesAttributes)
                .HasForeignKey(d => d.AttributeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Categorie__Attri__51EF2864");

            entity.HasOne(d => d.Hobby).WithMany(p => p.CategoriesHobbiesAttributes)
                .HasForeignKey(d => d.HobbyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Categorie__Hobby__50FB042B");
        });

        modelBuilder.Entity<CategoriesHobbiesSynonym>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC0795B2A8B1");

            entity.ToTable("Categories.Hobbies.Synonyms");

            entity.Property(e => e.HobbyId)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.SynonymId)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(d => d.Hobby).WithMany(p => p.CategoriesHobbiesSynonyms)
                .HasForeignKey(d => d.HobbyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Categorie__Hobby__54CB950F");
        });

        modelBuilder.Entity<CategoriesHobby>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07AEA3F765");

            entity.ToTable("Categories.Hobbies");

            entity.Property(e => e.Id).HasMaxLength(100);
            entity.Property(e => e.AddedBy)
                .IsRequired()
                .HasMaxLength(450)
                .HasDefaultValueSql("('1')");
            entity.Property(e => e.AddedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CategoryId)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.LastUpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.CategoriesHobbies)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Categorie__Categ__4E1E9780");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC0775491364");

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

        modelBuilder.Entity<UserHobbyPreference>(entity =>
        {
            entity.HasKey(e => e.PreferenceId).HasName("PK__User.Hob__E228496FF15E2D16");

            entity.ToTable("User.HobbyPreferences");

            entity.HasIndex(e => e.HobbyId, "idx_UserHobbyPreferences_HobbyId");

            entity.HasIndex(e => e.UserId, "idx_UserHobbyPreferences_UserId");

            entity.Property(e => e.DateProcessed)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.HobbyId).HasMaxLength(100);
            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne(d => d.Action).WithMany(p => p.UserHobbyPreferences)
                .HasForeignKey(d => d.ActionId)
                .HasConstraintName("FK__User.Hobb__Actio__5B78929E");

            entity.HasOne(d => d.Hobby).WithMany(p => p.UserHobbyPreferences)
                .HasForeignKey(d => d.HobbyId)
                .HasConstraintName("FK__User.Hobb__Hobby__5A846E65");
        });

        modelBuilder.Entity<UserHobbyPreferencesHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__User.Hob__4D7B4ABD9E6B34AF");

            entity.ToTable("User.HobbyPreferences.History");

            entity.HasIndex(e => e.HobbyId, "idx_UserHobbyPreferenceHistory_HobbyId");

            entity.HasIndex(e => e.UserId, "idx_UserHobbyPreferenceHistory_UserId");

            entity.Property(e => e.DateMovedToHistory)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DateProcessed).HasColumnType("datetime");
            entity.Property(e => e.HobbyId).HasMaxLength(100);
            entity.Property(e => e.UserId).IsRequired();

            entity.HasOne(d => d.Action).WithMany(p => p.UserHobbyPreferencesHistories)
                .HasForeignKey(d => d.ActionId)
                .HasConstraintName("FK__User.Hobb__Actio__61316BF4");

            entity.HasOne(d => d.Hobby).WithMany(p => p.UserHobbyPreferencesHistories)
                .HasForeignKey(d => d.HobbyId)
                .HasConstraintName("FK__User.Hobb__Hobby__603D47BB");

            entity.HasOne(d => d.Preference).WithMany(p => p.UserHobbyPreferencesHistories)
                .HasForeignKey(d => d.PreferenceId)
                .HasConstraintName("FK__User.Hobb__Prefe__5F492382");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
