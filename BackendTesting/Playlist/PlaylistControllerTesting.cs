using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lofi_backend.Controllers;
using lofi_backend.Data_Models.Enums;
using lofi_backend.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;

namespace Testing.PlaylistTests
{
    internal class PlaylistControllerTesting
    {
        private Mock<IPlaylistService> _mockPlaylistService;
        private PlaylistController _playlistController;

        [SetUp]
        public void SetUp()
        {
            _mockPlaylistService = new Mock<IPlaylistService>();
            _playlistController = new PlaylistController(_mockPlaylistService.Object);
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
            _mockPlaylistService.Setup(service => service.GetAllPlaylists()).Returns(testPlaylists);
            // Act
            var result = _playlistController.GetAllPlaylists() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            result.ShouldBeOfType<OkObjectResult>();

            var returnedPlaylists = result.Value as List<Playlist>;

            Assert.IsNotNull(returnedPlaylists);
            Assert.That(returnedPlaylists.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetPlaylistById_ReturnsPlaylist()
        {
            // Arrange
            var testPlaylist = new Playlist
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
            };
            _mockPlaylistService.Setup(service => service.GetPlaylistById("1")).Returns(testPlaylist);
            // Act
            var result = _playlistController.GetPlaylistById("1") as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            result.ShouldBeOfType<OkObjectResult>();

            var returnedPlaylist = result.Value as Playlist;

            Assert.IsNotNull(returnedPlaylist);
            Assert.That(returnedPlaylist.Id, Is.EqualTo("1"));
        }

        [Test]
        public void GetPlaylistById_ReturnsNotFound()
        {
            // Arrange
            _mockPlaylistService.Setup(service => service.GetPlaylistById("1")).Throws(new Exception());
            // Act
            var result = _playlistController.GetPlaylistById("1") as NotFoundResult;
            // Assert
            Assert.IsNotNull(result);
            result.ShouldBeOfType<NotFoundResult>();
        }

        [Test]
        public void CreatePlaylist_ReturnsCreatedPlaylist()
        {
            // Arrange
            var newPlaylist = new Playlist
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
            };
            _mockPlaylistService.Setup(service => service.CreatePlaylist(newPlaylist)).Returns(newPlaylist);
            // Act
            var result = _playlistController.CreatePlaylist(newPlaylist) as CreatedAtActionResult;
            // Assert
            Assert.IsNotNull(result);
            result.ShouldBeOfType<CreatedAtActionResult>();

            var createdPlaylist = result.Value as Playlist;
            Assert.IsNotNull(createdPlaylist);
            Assert.That(createdPlaylist.Id, Is.EqualTo("1"));
        }

        [Test]
        public void CreatePlaylist_PlaylistAlreadyExists_ReturnsBadRequest()
        {
            // Arrange
            var newPlaylist = new Playlist
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
            };
            _mockPlaylistService.Setup(service => service.CreatePlaylist(newPlaylist)).Throws(new Exception());
            // Act
            var result = _playlistController.CreatePlaylist(newPlaylist) as BadRequestResult;
            // Assert
            Assert.IsNotNull(result);
            result.ShouldBeOfType<BadRequestResult>();
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
            _mockPlaylistService.Setup(service => service.EditPlaylist(updatedPlaylist)).Returns(updatedPlaylist);
            // Act
            var result = _playlistController.EditPlaylist(updatedPlaylist) as OkObjectResult;
            // Assert
            Assert.IsNotNull(result);
            result.ShouldBeOfType<OkObjectResult>();
            var editedPlaylist = result.Value as Playlist;
            Assert.IsNotNull(editedPlaylist);
            Assert.That(editedPlaylist.Name, Is.EqualTo("Updated Playlist"));
        }

        [Test]
        public void EditPlaylist_PlaylistDoesNotExist_ReturnsBadRequest()
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
            _mockPlaylistService.Setup(service => service.EditPlaylist(updatedPlaylist)).Throws(new Exception());
            // Act
            var result = _playlistController.EditPlaylist(updatedPlaylist) as BadRequestResult;
            // Assert
            Assert.IsNotNull(result);
            result.ShouldBeOfType<BadRequestResult>();
        }

        [Test]
        public void DeletePlaylist_PlaylistExists_ReturnsNoContent()
        {
            // Arrange
            _mockPlaylistService.Setup(s => s.DeletePlaylist("1"))
                                .Returns(new Playlist { Id = "1" });

            // Act
            var result = _playlistController.DeletePlaylist("1") as NoContentResult;

            // Assert
            Assert.IsNotNull(result);
            result.ShouldBeOfType<NoContentResult>();
        }

        [Test]
        public void DeletePlaylist_PlaylistDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            _mockPlaylistService.Setup(s => s.DeletePlaylist("1"))
                                .Returns((Playlist)null);

            // Act
            var result = _playlistController.DeletePlaylist("1") as NotFoundObjectResult;

            // Assert
            Assert.IsNotNull(result);
            result.ShouldBeOfType<NotFoundObjectResult>();
        }
    }
}
