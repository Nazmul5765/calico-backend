using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using lofi_backend.Repository;
using lofi_backend.Service;
using Shouldly;


namespace Testing.MusicTesting
{
    internal class MusicServiceTests
    {
        private Mock<IMusicRepository> _mockRepo;
        private MusicService _musicService;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IMusicRepository>();
            _musicService = new MusicService(_mockRepo.Object);
        }

        [Test]
        public void GetAllMusics_ReturnsAllMusics()
        {
            // Arrange
            var musics = new List<Music>
            {
                new Music
                {
                    Id = 1,
                    Title = "Test Music",
                    Artist = "Test Artist",
                    Channel = "Test Channel",
                    Mood = lofi_backend.Data_Models.Enums.Mood.Romantic,
                    Genre = lofi_backend.Data_Models.Enums.Genre.LoFi,
                    URL = "Test URL"
                },

                new Music
                {
                    Id = 2,
                    Title = "Test Music 2",
                    Artist = "Test Artist 2",
                    Channel = "Test Channel 2",
                    Mood = lofi_backend.Data_Models.Enums.Mood.Focus,
                    Genre = lofi_backend.Data_Models.Enums.Genre.HipHop,
                    URL = "Test URL 2"
                }
            };
            _mockRepo.Setup(repo => repo.GetAllMusics()).Returns(musics);
            // Act
            var result = _musicService.GetAllMusics();
            // Assert
            result.ShouldBe(musics);
        }


        [Test]
        public void GetAllMusics_ReturnsEmptyList_WhenNoMusicsExist()
        {
            // Arrange
            var musics = new List<Music>();
            _mockRepo.Setup(repo => repo.GetAllMusics()).Returns(musics);
            // Act
            var result = _musicService.GetAllMusics();
            // Assert
            result.ShouldBeEmpty();
        }

        [Test]
        public void GetMusicById_ReturnsMusic()
        {
            // Arrange
            var music = new Music
            {
                Id = 1,
                Title = "Test Music",
                Artist = "Test Artist",
                Channel = "Test Channel",
                Mood = lofi_backend.Data_Models.Enums.Mood.Romantic,
                Genre = lofi_backend.Data_Models.Enums.Genre.LoFi,
                URL = "Test URL"
            };
            _mockRepo.Setup(repo => repo.GetMusicById(1)).Returns(music);
            // Act
            var result = _musicService.GetMusicById(1);
            // Assert
            result.ShouldBe(music);
        }

        [Test]
        public void GetMusicById_ReturnsNull_WhenMusicDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetMusicById(1)).Returns((Music)null);
            // Act
            var result = _musicService.GetMusicById(1);
            // Assert
            result.ShouldBeNull();
        }

        [Test]
        public void CreateMusic_ReturnsCreatedMusic()
        {
            // Arrange
            var musicToCreate = new Music
            {
                Title = "Test Music",
                Artist = "Test Artist",
                Channel = "Test Channel",
                Mood = lofi_backend.Data_Models.Enums.Mood.Romantic,
                Genre = lofi_backend.Data_Models.Enums.Genre.LoFi,
                URL = "Test URL"
            };
            var createdMusic = new Music
            {
                Id = 1,
                Title = "Test Music",
                Artist = "Test Artist",
                Channel = "Test Channel",
                Mood = lofi_backend.Data_Models.Enums.Mood.Romantic,
                Genre = lofi_backend.Data_Models.Enums.Genre.LoFi,
                URL = "Test URL"
            };
            _mockRepo.Setup(repo => repo.CreateMusic(musicToCreate)).Returns(createdMusic);
            // Act
            var result = _musicService.CreateMusic(musicToCreate);
            // Assert
            result.ShouldBe(createdMusic);
        }

        [Test]
        public void RemoveMusic_ReturnsRemovedMusic()
        {
            // Arrange
            var musicId = 1;
            _mockRepo.Setup(repo => repo.RemoveMusic(musicId));
            // Act
            var result = _musicService.RemoveMusic(musicId);
            // Assert
            _mockRepo.Verify(repo => repo.RemoveMusic(musicId), Times.Once);
        }
    }
}