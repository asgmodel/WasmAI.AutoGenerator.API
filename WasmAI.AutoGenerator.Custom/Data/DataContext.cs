


using AutoGenerator.Custom.Models;
using AutoGenerator.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace AutoGenerator.Custom.Data
{
    public class CodeDataContext : AutoIdentityDataContext<ApplicationCodeUser, IdentityRole, string>, ITAutoDbContext
    {
        public CodeDataContext(DbContextOptions options) : base(options)
        {


        }

        // Add properties like DbSet for your models


        public DbSet<Section> Sections { get; set; }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Folder> Folders { get; set; }

        public DbSet<FileEntity> Files { get; set; }

        public DbSet<CodeGenerator> CodeGenerators { get; set; }

        public DbSet<CodeHistory> CodeHistories { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationCodeUser>()
               .HasMany(u => u.Sections)
               .WithOne(s => s.ApplicationCodeUser)
               .HasForeignKey(s => s.ApplicationCodeUserId);

            // ApplicationCodeUser <-> Projects (One-to-Many - Less common directly, usually via Sections)
            // If a Project must belong to a Section which belongs to a User, remove this direct link
            // Assuming direct link is intended based on your model definition
            modelBuilder.Entity<ApplicationCodeUser>()
                 .HasMany(u => u.Projects)
                 .WithOne() // No navigation property back in Project for User (optional)
                 .HasForeignKey("ApplicationCodeUserId"); // Assuming FK is ApplicationCodeUserId in Project

            // Section <-> Projects (One-to-Many)
            modelBuilder.Entity<Section>()
                .HasMany(s => s.Projects)
                .WithOne(p => p.Section)
                .HasForeignKey(p => p.SectionId);

            // Project <-> Folders (One-to-Many)
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Folders)
                .WithOne(f => f.Project)
                .HasForeignKey(f => f.ProjectId);

            // Folder <-> Files (One-to-Many)
            modelBuilder.Entity<Folder>()
                .HasMany(f => f.Files)
                .WithOne(fe => fe.Folder)
                .HasForeignKey(fe => fe.FolderId);
            // CodeGenerator <-> CodeHistory (One-to-Many)
            modelBuilder.Entity<CodeGenerator>()
                .HasMany(cg => cg.CodeHistories)
                .WithOne(ch => ch.CodeGenerator)
                .HasForeignKey(ch => ch.CodeGeneratorId);



            // --- Configure Entity Properties ---
            modelBuilder.Entity<Section>().HasKey(s => s.Id);
            modelBuilder.Entity<Project>().HasKey(p => p.Id);
            modelBuilder.Entity<Folder>().HasKey(f => f.Id);
            modelBuilder.Entity<FileEntity>().HasKey(fe => fe.Id);
            modelBuilder.Entity<CodeGenerator>().HasKey(cg => cg.Id);
            modelBuilder.Entity<CodeHistory>().HasKey(ch => ch.Id);

        }



    }
}