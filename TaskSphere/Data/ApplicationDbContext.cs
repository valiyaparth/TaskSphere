using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TaskSphere.Models;

namespace TaskSphere.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
            public DbSet<Models.Task> Tasks { get; set; }
            public DbSet<User> Users { get; set; }
            public DbSet<Team> Teams { get; set; }
            public DbSet<Project> Projects { get; set; }

            public DbSet<ProjectMember> ProjectMembers { get; set; }
            public DbSet<ProjectTeam> ProjectTeams { get; set; }
            public DbSet<TaskUser> TaskUsers { get; set; }
            public DbSet<TeamMember> TeamMembers { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            //Creators:

            //Task
            modelBuilder.Entity<Models.Task>()
                .HasOne(t => t.Creator)
                .WithMany()
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
            //Team
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Creator)
                .WithMany()
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
            //Project
            modelBuilder.Entity<Project>()
                .HasOne(p=>p.Creator)
                .WithMany()
                .HasForeignKey(p=>p.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            //enums
            //modelBuilder.Entity<Models.Task>()
            //.Property(t => t.Status)
            //.HasConversion<string>();

            //modelBuilder.Entity<ProjectMember>()
            //    .Property(p => p.Role)
            //    .HasConversion<string>();

            //modelBuilder.Entity<TeamMember>()
            //    .Property(t => t.Role)
            //    .HasConversion<string>();



            //One-to-Many:

            //Project -> Task
            modelBuilder.Entity<Project>()
                .HasMany(p=>p.Tasks)
                .WithOne(t=>t.Project)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            //Team -> Task 
            modelBuilder.Entity<Team>()
                .HasMany(t => t.Tasks)
                .WithOne(p => p.Team)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.Cascade);


            //Many-to-Many:

            //Project <-> User
            modelBuilder.Entity<ProjectMember>()
                .HasKey(pm => new { pm.ProjectId, pm.UserId });

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.Members)
                .HasForeignKey(pm => pm.ProjectId);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(pm => pm.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(pm => pm.UserId);

            //Project <-> Team
            modelBuilder.Entity<ProjectTeam>()
                .HasKey(pt => new { pt.ProjectId, pt.TeamId });

            modelBuilder.Entity<ProjectTeam>()
                .HasOne(pt => pt.Project)
                .WithMany(p => p.Teams)
                .HasForeignKey(pt => pt.ProjectId);

            modelBuilder.Entity<ProjectTeam>()
                .HasOne(pt => pt.Team)
                .WithMany(t => t.Projects)
                .HasForeignKey(pt => pt.TeamId);

            //Task <-> User
            modelBuilder.Entity<TaskUser>()
                .HasKey(tu => new { tu.TaskId, tu.UserId });

            modelBuilder.Entity<TaskUser>()
                .HasOne(tu => tu.Task)
                .WithMany(t => t.Assignee)
                .HasForeignKey(tu => tu.TaskId);

            modelBuilder.Entity<TaskUser>()
                .HasOne(tu => tu.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(tu => tu.UserId);

            // Team <-> User
            modelBuilder.Entity<TeamMember>()
                .HasKey(tm => new { tm.TeamId, tm.UserId });

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.TeamMembers)
                .HasForeignKey(tm => tm.TeamId);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.User)
                .WithMany(u => u.Teams)
                .HasForeignKey(tm => tm.UserId);



        }
    }
}
