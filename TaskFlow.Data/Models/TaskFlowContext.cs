using TaskFlow.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TaskFlow.Data
{
    public class TaskFlowContext : DbContext
    {
        public TaskFlowContext() { }
        public TaskFlowContext(DbContextOptions<TaskFlowContext> options) : base(options) { }
        public DbSet<tblStatus> Status { get; set; }
        public DbSet<tblUserRole> UserRole { get; set; }
        public DbSet<tblSystemUser> SystemUser { get; set; }
        public DbSet<tblProject> Project { get; set; }
        public DbSet<tblCategory> Category { get; set; }
        public DbSet<tblTodoItem> TodoItem { get; set; }
        public DbSet<tblSystemLog> SystemLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
           .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
           .AddJsonFile("appsettings.json")
           .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
