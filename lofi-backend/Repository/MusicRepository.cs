using lofi_backend.Database;
using lofi_backend.Models;
using Microsoft.IdentityModel.Tokens;



namespace lofi_backend.Repository
{
    public interface IMusicRepository
    {

        public List<Music> GetAllMusics();
        public Music GetMusicById(int id);
        public Music CreateMusic(Music music);
        public Music RemoveMusic(int id);
    }

    public class MusicRepository : IMusicRepository
    {
        private readonly LoFiDbContext _db;
        public MusicRepository(LoFiDbContext dbContext)
        {
            _db = dbContext;
        }
        public List<Music> GetAllMusics()
        {
            if (_db.Music.ToList().IsNullOrEmpty()) throw new Exception("No musics found");
            return _db.Music.ToList();
        }
        public Music GetMusicById(int id)
        {
            return _db.Music.ToList().First(m => m.Id == id) ?? throw new Exception("Music not found");
        }
        public Music CreateMusic(Music music)
        {
            if (_db.Music.Contains(music)) throw new Exception("Music exists");
            var newMusic = _db.Music.Add(music).Entity;
            Console.WriteLine(newMusic.Title + " has been saved");
            _db.SaveChanges();
            return newMusic;
        }
        public Music RemoveMusic(int id)
        {
            var deletedMusic = _db.Music.First(m => m.Id == id);
            if (deletedMusic == null)
                throw new Exception("Music does not exist");
            _db.Music.Remove(deletedMusic);
            _db.SaveChanges();
            return deletedMusic;
        }
    }
}
