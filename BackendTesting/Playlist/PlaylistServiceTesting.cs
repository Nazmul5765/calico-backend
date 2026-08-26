using lofi_backend.Data_Models.Enums;
using lofi_backend.Repository;
using lofi_backend.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing.PlaylistTests
{
    internal class PlaylistServiceTesting
    {
        private Mock<IPlaylistRepository> _mockRepo;
        private PlaylistService _playlistService;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IPlaylistRepository>();
            _playlistService = new PlaylistService(_mockRepo.Object);
        }

        [Test]
        public void GetAllPlaylists_ReturnsListOfPlaylists()
        {
            // Arrange
            var testPlaylists = new List<Playlist>
            {
                new Playlist
                {
                    Id = "1",
                    Name = "Test Playlist 1",
                    Mood = Mood.Study,
                    Genre = Genre.Ambient,
                    Songs = new List<Music>
                    {
                        new Music { Id = 1, Title = "Track 1", Artist = "Artist 1", Channel = "testChannel", Genre = Genre.Ambient, Mood = Mood.Chill, URL = "www.testurl" },
                        new Music { Id = 2, Title = "Track 2", Artist = "Artist 2", Channel = "testChannel2", Genre = Genre.Chill, Mood = Mood.Relax, URL = "www.testurl2"}
                    }
                },
                new Playlist
                {
                    Id = "2",
                    Name = "Test Playlist 2",
                    Mood = Mood.Chill,
                    Genre = Genre.LoFi,
                    Songs = new List<Music>
                    {
                        new Music { Id = 3, Title = "Track 3", Artist = "Artist 3", Channel = "testChannel3", Genre = Genre.LoFi, Mood = Mood.Chill, URL = "www.testurl3" },
                        new Music { Id = 4, Title = "Track 4", Artist = "Artist 4", Channel = "testChannel4", Genre = Genre.LoFi, Mood = Mood.Relax, URL = "www.testurl4"}
                    }
                }
            };
            _mockRepo.Setup(repo => repo.GetAllPlaylists()).Returns(testPlaylists);
            // Act
            var result = _playlistService.GetAllPlaylists();
            // Assert
            Assert.That(result, Is.EqualTo(testPlaylists));
        }



        [Test]
        public void GetPlaylistById_ReturnsPlaylist()
        {
            // Arrange
            var testPlaylist = new Playlist
            {
                Id = "1",
                Name = "Test Playlist",
                Mood = Mood.Study,
                Genre = Genre.Ambient,
                Songs = new List<Music>
                {
                    new Music { Id = 1, Title = "Track 1", Artist = "Artist 1", Channel = "testChannel", Genre = Genre.Ambient, Mood = Mood.Chill, URL = "www.testurl" },
                    new Music { Id = 2, Title = "Track 2", Artist = "Artist 2", Channel = "testChannel2", Genre = Genre.Chill, Mood = Mood.Relax, URL = "www.testurl2"}
                }
            };
            _mockRepo.Setup(repo => repo.GetPlaylistById("1")).Returns(testPlaylist);
            // Act
            var result = _playlistService.GetPlaylistById("1");
            // Assert
            Assert.That(result, Is.EqualTo(testPlaylist));
        }

        [Test]
        public void CreatePlaylist_ReturnsCreatedPlaylist()
        {
            // Arrange
            var newPlaylist = new Playlist
            {
                Id = "1",
                Name = "New Playlist",
                Mood = Mood.Chill,
                Genre = Genre.LoFi,
                Songs = new List<Music>
                {
                    new Music { Id = 1, Title = "Track 1", Artist = "Artist 1", Channel = "testChannel", Genre = Genre.LoFi, Mood = Mood.Chill, URL = "www.testurl" },
                    new Music { Id = 2, Title = "Track 2", Artist = "Artist 2", Channel = "testChannel2", Genre = Genre.LoFi, Mood = Mood.Relax, URL = "www.testurl2"}
                }
            };
            _mockRepo.Setup(repo => repo.CreatePlaylist(newPlaylist)).Returns(newPlaylist);
            // Act
            var result = _playlistService.CreatePlaylist(newPlaylist);
            // Assert
            Assert.That(result, Is.EqualTo(newPlaylist));
        }

        [Test]
        public void EditPlaylist_ReturnsUpdatedPlaylist()
        {
            // Arrange
            var updatedPlaylist = new Playlist
            {
                Id = "1",
                Name = "Updated Playlist",
                Mood = Mood.Study,
                Genre = Genre.Ambient,
                Songs = new List<Music>
                {
                    new Music { Id = 1, Title = "Track 1", Artist = "Artist 1", Channel = "testChannel", Genre = Genre.Ambient, Mood = Mood.Chill, URL = "www.testurl" },
                    new Music { Id = 2, Title = "Track 2", Artist = "Artist 2", Channel = "testChannel2", Genre = Genre.Chill, Mood = Mood.Relax, URL = "www.testurl2"}
                }
            };
            _mockRepo.Setup(repo => repo.EditPlaylist(updatedPlaylist)).Returns(updatedPlaylist);
            // Act
            var result = _playlistService.EditPlaylist(updatedPlaylist);
            // Assert
            Assert.That(result, Is.EqualTo(updatedPlaylist));
            _mockRepo.Verify(repo => repo.EditPlaylist(updatedPlaylist), Times.Once);
        }

        [Test]
        public void DeletePlaylist_ReturnsDeletedPlaylist()
        {
            // Arrange
            var deletedPlaylist = new Playlist
            {
                Id = "1",
                Name = "Deleted Playlist",
                Mood = Mood.Chill,
                Genre = Genre.LoFi,
                Songs = new List<Music>
                {
                    new Music { Id = 1, Title = "Track 1", Artist = "Artist 1", Channel = "testChannel", Genre = Genre.LoFi, Mood = Mood.Chill, URL = "www.testurl" },
                    new Music { Id = 2, Title = "Track 2", Artist = "Artist 2", Channel = "testChannel2", Genre = Genre.LoFi, Mood = Mood.Relax, URL = "www.testurl2"}
                }
            };
            _mockRepo.Setup(repo => repo.DeletePlaylist("1")).Returns(deletedPlaylist);
            // Act
            var result = _playlistService.DeletePlaylist("1");
            // Assert
            Assert.That(result, Is.EqualTo(deletedPlaylist));
            _mockRepo.Verify(repo => repo.DeletePlaylist("1"), Times.Once);
        }
    }
}
