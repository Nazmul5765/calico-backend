using lofi_backend.Data_Models;
using lofi_backend.Data_Models.Enums;
using lofi_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace lofi_backend.Database
{
    public class LoFiDbContext : DbContext
    {
        public DbSet<Music> Music { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<TaskTimer> Timers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<UserData> Users { get; set; }

        public LoFiDbContext(DbContextOptions<LoFiDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") is "Development")
            {
                modelBuilder.Entity<UserData>().HasData(
                    new UserData
                    {
                        Id = "test1",
                        Username = "studyowl",
                        FirstName = "Emma",
                        LastName = "Thompson",
                        Email = "samuelgdukes@gmail.com",
                        DateOfBirth = new DateTime(1988, 4, 12), 
                        Gender = Gender.Female, 

                    },
                    new UserData
                    {
                        Id = "test2",
                        Username = "lofilover",
                        FirstName = "Matthew",
                        LastName = "Painter",
                        Email = "matthew.p@example.com",
                        DateOfBirth = new DateTime(1995, 11, 3),
                        Gender = Gender.Male
                    },
                    new UserData
                    {
                        Id = "test3",
                        Username = "nightwave",
                        FirstName = "Sofia",
                        LastName = "Nguyen",
                        Email = "s.nguyen@example.com",
                        DateOfBirth = new DateTime(2000, 9, 5),
                        Gender = Gender.NonBinary
                    },
                    new UserData
                    {
                        Id = "cdc75697-df68-4110-8e74-b37244ec9c36",
                        Username = "nazmul5765",
                        FirstName = "Nazmul",
                        LastName = "Hussain",
                        DateOfBirth = new DateTime(1991, 11, 08),
                        Email = "nazmulhussain@hotmail.co.uk",
                        Gender = Gender.Male,
                        IsAdmin = true,
                    }
                );

                modelBuilder.Entity<Project>().HasData(
                    new Project
                    {
                        Id = 1,
                        UserId = "test1",
                        Name = "Portfolio Website",
                        StartDate = new DateTime(2026, 1, 6, 13, 0, 0),
                        EndDate = new DateTime(2026, 1, 7, 13, 0, 0),
                        Timers = new List<TaskTimer>()
                    },
                    new Project
                    {
                        Id = 2,
                        UserId = "test2",
                        Name = "English Essay",
                        StartDate = new DateTime(2026, 6, 16, 12, 0, 0),
                        EndDate = new DateTime(2026, 6, 20, 16, 0, 0),
                        Timers = new List<TaskTimer>()
                    },
                    new Project
                    {
                        Id = 3,
                        UserId = "test3",
                        Name = "Sewing skirt",
                        StartDate = new DateTime(2026, 6, 17, 10, 0, 0),
                        EndDate = new DateTime(2026, 6, 17, 17, 0, 0),
                        Timers = new List<TaskTimer>()
                    },
                    new Project
                    {
                        Id = 4,
                        UserId = "test1",
                        Name = "Apply for job",
                        StartDate = new DateTime(2026, 6, 22, 13, 0, 0),
                        EndDate = new DateTime(2026, 6, 22, 14, 0, 0),
                        Timers = new List<TaskTimer>()
                    }
                );

                modelBuilder.Entity<TaskTimer>().HasData(
                    new TaskTimer
                    {
                        Id = 1,
                        ProjectId = 1,
                        DateCreated = new DateTime(2026, 1, 6, 13, 0, 0),
                        DateUpdated = new DateTime(2026, 1, 6, 14, 0, 0),
                        Duration = 3600,
                        IsActive = false
                    },
                    new TaskTimer
                    {
                        Id = 2,
                        ProjectId = 2,
                        DateCreated = new DateTime(2026, 6, 16, 12, 0, 0),
                        DateUpdated = new DateTime(2026, 6, 16, 14, 30, 0),
                        Duration = 9000,
                        IsActive = false
                    },
                    new TaskTimer
                    {
                        Id = 3,
                        ProjectId = 3,
                        DateCreated = new DateTime(2026, 6, 17, 10, 0, 0),
                        DateUpdated = new DateTime(2026, 6, 17, 12, 15, 0),
                        Duration = 8100,
                        IsActive = false
                    },
                    new TaskTimer
                    {
                        Id = 4,
                        ProjectId = 4,
                        DateCreated = new DateTime(2026, 6, 22, 13, 0, 0),
                        DateUpdated = new DateTime(2026, 6, 22, 13, 45, 0),
                        Duration = 2700,
                        IsActive = false
                    }
                );

                //modelBuilder.Entity<Music>().HasData(
                //    new Music { Id = 1, Title = "Best of lofi hip hop 2021", Artist = "Various Artists", Channel = "Lofi Girl", Mood = Mood.Study, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=n61ULEU7CO0" },
                //    new Music { Id = 2, Title = "1 A.M Study Session", Artist = "Various Artists", Channel = "Lofi Girl", Mood = Mood.Study, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=lTRiuFIWV54" },
                //    new Music { Id = 3, Title = "Winter lofi mix", Artist = "Various Artists", Channel = "Lofi Girl", Mood = Mood.Relax, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=S-4hwfyK-XQ" },
                //    new Music { Id = 4, Title = "Lofi beats to do absolutely nothing to", Artist = "Various Artists", Channel = "Lofi Girl", Mood = Mood.Chill, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=M8J9zHyyUYc" },
                //    new Music { Id = 5, Title = "Study with me Pomodoro lofi focus music", Artist = "Various Artists", Channel = "Lofi Girl", Mood = Mood.Focus, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=53gNFOqDFcE" },

                //    new Music { Id = 6, Title = "Chillhop Essentials Spring 2018", Artist = "Various Artists", Channel = "Chillhop Music", Mood = Mood.Happy, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=HRNcojzOJVk" },
                //    new Music { Id = 7, Title = "Chillhop Essentials Summer 2018", Artist = "Various Artists", Channel = "Chillhop Music", Mood = Mood.Chill, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=TTXFKD7fMlE" },
                //    new Music { Id = 8, Title = "Chillhop Essentials Fall 2018", Artist = "Various Artists", Channel = "Chillhop Music", Mood = Mood.Calm, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=M8HDvTuctOU" },
                //    new Music { Id = 9, Title = "Chillhop Essentials Winter 2018", Artist = "Various Artists", Channel = "Chillhop Music", Mood = Mood.Relax, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=Rhomn5Um9dg" },
                //    new Music { Id = 10, Title = "Chillhop Essentials Fall 2016", Artist = "Various Artists", Channel = "Chillhop Music", Mood = Mood.Chill, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=5jZyM6-k50o" },

                //    new Music { Id = 11, Title = "Chillhop Essentials Winter 2020", Artist = "Various Artists", Channel = "Chillhop Music", Mood = Mood.Calm, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=i_cV8B2pjqk" },
                //    new Music { Id = 12, Title = "Chillhop Daydreams", Artist = "Various Artists", Channel = "Chillhop Music", Mood = Mood.Relax, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=kEPakJDkTOk" },
                //    new Music { Id = 13, Title = "5 Hours Chill Lofi Hip-Hop Mix 2018", Artist = "Various Artists", Channel = "Lofi Mix", Mood = Mood.Study, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=kNZjFeqw_28" },
                //    new Music { Id = 14, Title = "Chill music for work", Artist = "Various Artists", Channel = "Lofi Work", Mood = Mood.Focus, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=JCKBaJDRMw4" },
                //    new Music { Id = 15, Title = "Cozy spring lofi chill music", Artist = "Various Artists", Channel = "Lofi Girl", Mood = Mood.Relax, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=fsPRybb-xXg" },

                //    new Music { Id = 16, Title = "Best of lofi 2018", Artist = "Various Artists", Channel = "Lofi Girl", Mood = Mood.Study, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=-FlxM_0S2lA" },
                //    new Music { Id = 17, Title = "Tomorrow", Artist = "Various Artists", Channel = "Lofi Girl", Mood = Mood.Calm, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=J8nTKJ-dP00" },
                //    new Music { Id = 18, Title = "Chill Study Music Playlist", Artist = "Various Artists", Channel = "Lofi Beats", Mood = Mood.Focus, Genre = Genre.LoFi, URL = "https://www.youtube.com/watch?v=2tr6iYIvL3k" },
                //    new Music
                //    {
                //        Id = 19,
                //        Title = "Relax LoFi Study Mix",
                //        Artist = "LoFi Beats",
                //        Channel = "LoFi Beats",
                //        Mood = Mood.Study,
                //        Genre = Genre.LoFi,
                //        URL = "https://www.youtube.com/watch?v=JdqL89ZZwFw"
                //    },
                //    new Music
                //    {
                //        Id = 20,
                //        Title = "Late Night LoFi Coding",
                //        Artist = "Chill Programmer",
                //        Channel = "Coding Beats",
                //        Mood = Mood.Focus,
                //        Genre = Genre.LoFi,
                //        URL = "https://www.youtube.com/watch?v=dQi-ofZmrPw"
                //    }
                //);

                modelBuilder.Entity<Playlist>().HasData(
                    new Playlist
                    {
                        Id = "playlist1",
                        Name = "Morning Focus",
                        Mood = Mood.Focus,
                        Genre = Genre.LoFi,
                        Songs = new List<Music>()

                    },

                    new Playlist
                    {
                        Id = "playlist2",
                        Name = "Relax Evening",
                        Mood = Mood.Relax,
                        Genre = Genre.Chill,
                        Songs = new List<Music>()
                    },

                    new Playlist
                    {
                        Id = "playlist3",
                        Name = "Sleep Time",
                        Mood = Mood.Sleep,
                        Genre = Genre.LoFi,
                        Songs = new List<Music>()
                    }
                );
            }
        }
    }
}
