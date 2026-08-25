using lofi_backend.Models;
using lofi_backend.Repository;

namespace lofi_backend.Service
{

    public interface IPlaylistService
    {
        public IEnumerable<Playlist> GetAllPlaylists();
        public Playlist GetPlaylistById(string id);
        public Playlist CreatePlaylist(Playlist playlist);
        public Playlist EditPlaylist(Playlist playlist);
        public Playlist DeletePlaylist(string id);
    }

    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _repository;

        public PlaylistService(IPlaylistRepository repository)
        {
            _repository = repository;
        }


        public IEnumerable<Playlist> GetAllPlaylists()
        {
            try
            {
                return _repository.GetAllPlaylists();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching playlists: {ex.Message}");
                throw;
            }
        }

        public Playlist GetPlaylistById(string id)
        {
            try
            {
                return _repository.GetPlaylistById(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching playlist with Id {id}: {ex.Message}");
                throw;
            }
        }
        public Playlist CreatePlaylist(Playlist playlist)
        {
            try
            {
                return _repository.CreatePlaylist(playlist);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating playlist: {ex.Message}");
                throw;
            }
        }
        public Playlist EditPlaylist(Playlist playlist)
        {
            try
            {
                return _repository.EditPlaylist(playlist);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing playlist with Id {playlist.Id}: {ex.Message}");
                throw;
            }
        }
        public Playlist DeletePlaylist(string id)
        {
            try
            {
                return _repository.DeletePlaylist(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting playlist with Id {id}: {ex.Message}");
                throw;
            }

        }
    }
}
