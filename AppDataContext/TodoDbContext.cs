using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ExampleAPI.Models;

namespace ExampleAPI.AppDataContext
{

    // TodoDbContext class inherits from DbContext
     public class TodoDbContext : DbContext
     {

        // DbSettings field to store the connection string
        //  private readonly DbSettings _dbsettings;
         private readonly string _connectionString;

            // Constructor to inject the DbSettings model
         public TodoDbContext(IOptions<DbSettings> dbSettings)
         {
            // Memuat file .env jika ada
            DotNetEnv.Env.Load();
            
            // Mengambil environment variables
            _connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                                $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                                $"Username={Environment.GetEnvironmentVariable("DB_USERNAME")};" +
                                $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                                $"Database={Environment.GetEnvironmentVariable("DB_DATABASE")};";
         }


        // DbSet property to represent the Todo table
         public DbSet<Todo> Todos { get; set; }

         // Configuring the database provider and connection string

         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         {
            optionsBuilder.UseNpgsql(_connectionString);
         }

            // Configuring the model for the Todo entity
         protected override void OnModelCreating(ModelBuilder modelBuilder)
         {
             modelBuilder.Entity<Todo>()
                 .ToTable("TodoAPI")
                 .HasKey(x => x.Id);
         }
     }
}