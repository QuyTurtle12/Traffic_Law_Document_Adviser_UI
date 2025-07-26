using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BussinessObject;

public partial class TrafficLawDocumentDbContext : DbContext
{
    public TrafficLawDocumentDbContext()
    {
    }

    public TrafficLawDocumentDbContext(DbContextOptions<TrafficLawDocumentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChatDocument> ChatDocuments { get; set; }

    public virtual DbSet<ChatHistory> ChatHistories { get; set; }

    public virtual DbSet<DocumentCategory> DocumentCategories { get; set; }

    public virtual DbSet<DocumentTag> DocumentTags { get; set; }

    public virtual DbSet<DocumentTagMap> DocumentTagMaps { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<LawDocument> LawDocuments { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatDocument>(entity =>
        {
            entity.HasIndex(e => e.LawDocumentId, "IX_ChatDocuments_LawDocumentId");

            entity.HasIndex(e => e.UserId, "IX_ChatDocuments_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.LawDocument).WithMany(p => p.ChatDocuments).HasForeignKey(d => d.LawDocumentId);

            entity.HasOne(d => d.User).WithMany(p => p.ChatDocuments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ChatHistory>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_ChatHistories_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastUpdatedTime).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.ChatHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DocumentCategory>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastUpdatedTime).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<DocumentTag>(entity =>
        {
            entity.HasIndex(e => e.ParentTagId, "IX_DocumentTags_ParentTagId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastUpdatedTime).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ParentTag).WithMany(p => p.InverseParentTag).HasForeignKey(d => d.ParentTagId);
        });

        modelBuilder.Entity<DocumentTagMap>(entity =>
        {
            entity.HasIndex(e => e.DocumentId, "IX_DocumentTagMaps_DocumentId");

            entity.HasIndex(e => e.DocumentTagId, "IX_DocumentTagMaps_DocumentTagId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Document).WithMany(p => p.DocumentTagMaps).HasForeignKey(d => d.DocumentId);

            entity.HasOne(d => d.DocumentTag).WithMany(p => p.DocumentTagMaps).HasForeignKey(d => d.DocumentTagId);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasIndex(e => e.ChatHistory, "IX_Feedbacks_ChatHistory");

            entity.HasIndex(e => e.UserId, "IX_Feedbacks_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Aianswer).HasColumnName("AIAnswer");
            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastUpdatedTime).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ChatHistoryNavigation).WithMany(p => p.Feedbacks).HasForeignKey(d => d.ChatHistory);

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<LawDocument>(entity =>
        {
            entity.HasIndex(e => e.CategoryId, "IX_LawDocuments_CategoryId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastUpdatedTime).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Category).WithMany(p => p.LawDocuments).HasForeignKey(d => d.CategoryId);
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_News_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EmbeddedNewsId).HasColumnName("embeddedNewsId");
            entity.Property(e => e.LastUpdatedTime).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.News).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastUpdatedTime).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
