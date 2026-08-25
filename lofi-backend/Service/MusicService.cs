using lofi_backend.Models;
using lofi_backend.Repository;

namespace lofi_backend.Service
{
    public interface IMusicService
    {
        public List<Music> GetAllMusics();
        public Music GetMusicById(int id);
        public Music CreateMusic(Music music);
        public Music RemoveMusic(int id);
    }

    public class MusicService : IMusicService
    {
        private readonly IMusicRepository _repository;
        public MusicService(IMusicRepository repository)
        {
            _repository = repository;
        }
        public List<Music> GetAllMusics()
        {
            return _repository.GetAllMusics();
        }
        public Music GetMusicById(int id)
        {
            return _repository.GetMusicById(id);
        }
        public Music CreateMusic(Music music)
        {
            return _repository.CreateMusic(music);
        }
        public Music RemoveMusic(int id)
        {
            return _repository.RemoveMusic(id);
        }
    }
    }
