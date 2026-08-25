using lofi_backend.Database;
using lofi_backend.Models;

namespace lofi_backend.Repository
{

    public interface IPlaylistRepository
    {
        public IEnumerable<Playlist> GetAllPlaylists();
        public Playlist GetPlaylistById(string id);
        public Playlist CreatePlaylist(Playlist playlist);
        public Playlist EditPlaylist(Playlist playlist);
        public Playlist DeletePlaylist(string id);
    }

    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly LoFiDbContext _db;

        public PlaylistRepository(LoFiDbContext dbContext)
        {
            _db = dbContext;
        }

        public IEnumerable<Playlist> GetAllPlaylists()
        { 
            return _db.Playlists.ToList();
        }
        public Playlist GetPlaylistById(string id)
        {
            return _db.Playlists.FirstOrDefault(p => p.Id == id);
        }
        public Playlist CreatePlaylist(Playlist playlist)
        {
            if (_db.Playlists.Any(p => p.Id == playlist.Id))
            {
                throw new Exception("Playlist with the same Id already exists.");
            }

            var newPlaylist = _db.Playlists.Add(playlist).Entity;
            _db.SaveChanges();
            return newPlaylist;
        }

        public Playlist EditPlaylist(Playlist playlist)
        { 
            if(!_db.Playlists.Any(p => p.Id == playlist.Id))
            {
                throw new Exception("Playlist not found.");
            }
            var editPlaylist = _db.Playlists.Update(playlist).Entity;
            _db.SaveChanges();
            return editPlaylist;

        }

        public Playlist DeletePlaylist(string id)
        {
            var deletePlaylist = _db.Playlists.FirstOrDefault(p => p.Id == id);

            if (deletePlaylist == null)
            {
                throw new Exception("Playlist not found.");
            }

            _db.Playlists.Remove(deletePlaylist);
            _db.SaveChanges();
            return deletePlaylist;

        }
    }
}
