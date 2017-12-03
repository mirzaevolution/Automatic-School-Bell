using System.Data.Entity;
using CoreLib.Models;
namespace CoreLib.DataAccess
{
    public class DataContext:DbContext
    {
        public DataContext() { }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<RepeatedSchedule> RepeateadSchedules { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DataContext>());
        }
    }
}
