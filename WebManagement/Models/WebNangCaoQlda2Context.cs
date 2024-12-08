using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APIwebmoi.Models;

public partial class WebNangCaoQlda2Context : DbContext
{
    public WebNangCaoQlda2Context()
    {
    }

    public WebNangCaoQlda2Context(DbContextOptions<WebNangCaoQlda2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Column> Columns { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<NguoiDung> NguoiDungs { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<UserTask> UserTasks { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-681BB00;Database=webNangCaoQLDA2;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Column>(entity =>
        {
            entity.HasKey(e => e.id_column).HasName("PK__columns__2675320D6A8861F5");

            entity.ToTable("columns");

            entity.Property(e => e.id_column)
                .ValueGeneratedNever()
                .HasColumnName("id_column");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.IdProject).HasColumnName("id_project");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.HasOne(d => d.IdProjectNavigation).WithMany(p => p.Columns)
                .HasForeignKey(d => d.IdProject)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__columns__id_proj__3F466844");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.IdComment).HasName("PK__Comment__7E14AC85C7183EA2");

            entity.ToTable("Comment");

            entity.Property(e => e.IdComment)
                .ValueGeneratedNever()
                .HasColumnName("id_comment");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.IdUser)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id_user");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Comments)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Comment__id_user__44FF419A");
        });

        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("PK__nguoiDun__D2D14637258C24E0");

            entity.ToTable("nguoiDung");

            entity.Property(e => e.IdUser)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id_user");
            entity.Property(e => e.Avatar)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Pass)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("pass");
            entity.Property(e => e.Username)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("username");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.NguoiDungs)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__nguoiDung__id_ro__398D8EEE");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.id_project).HasName("PK__Project__274AE1B377905048");

            entity.ToTable("Project");

            entity.Property(e => e.id_project)
                .ValueGeneratedNever()
                .HasColumnName("id_project");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Mota).HasColumnName("mota");
            entity.Property(e => e.OwnerIds)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ownerIds");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");

            entity.HasOne(d => d.OwnerIdsNavigation).WithMany(p => p.Projects)
                .HasForeignKey(d => d.OwnerIds)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Project__ownerId__3C69FB99");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.id_task).HasName("PK__Task__C1D2C6175C5A9790");

            entity.ToTable("Task");

            entity.Property(e => e.id_task)
                .ValueGeneratedNever()
                .HasColumnName("id_task");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Deadline)
                .HasColumnType("datetime")
                .HasColumnName("deadline");
            entity.Property(e => e.id_column).HasColumnName("id_column");
            entity.Property(e => e.Mota).HasColumnName("mota");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
            entity.Property(e => e.UpdateAt)
                .HasColumnType("datetime")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.IdColumnNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.id_column)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Task__id_column__4222D4EF");
        });

        modelBuilder.Entity<UserTask>(entity =>
        {
            entity.HasKey(e => e.IdUserTask).HasName("PK__User_Tas__53FD027BB02D97D0");

            entity.ToTable("User_Task");

            entity.Property(e => e.IdUserTask).HasColumnName("id_user_task");
            entity.Property(e => e.IdTask).HasColumnName("id_task");
            entity.Property(e => e.IdUser)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("id_user");

            entity.HasOne(d => d.IdTaskNavigation).WithMany(p => p.UserTasks)
                .HasForeignKey(d => d.IdTask)
                .HasConstraintName("FK__User_Task__id_ta__48CFD27E");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.UserTasks)
                .HasForeignKey(d => d.IdUser)
                .HasConstraintName("FK__User_Task__id_us__47DBAE45");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("PK__vaiTro__3D48441DB584590E");

            entity.ToTable("vaiTro");

            entity.Property(e => e.IdRole)
                .ValueGeneratedNever()
                .HasColumnName("id_role");
            entity.Property(e => e.RoleName)
                .HasMaxLength(10)
                .HasColumnName("role_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
