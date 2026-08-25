using lofi_backend.Models;
using lofi_backend.Repository;

namespace lofi_backend.Service
{
    public interface IUserService
    {
        public UserData GetUserById(string id);
        public Task<UserData> CreateUser(UserWithPassword user);
        public UserData EditUser(UserData user);
        public UserData RemoveUser(string id);
        List<UserData> GetAllUsers();
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }
        
        public UserData GetUserById(string id)
        {
            return _repository.FetchUserById(id);
        }

        public List<UserData> GetAllUsers()
        {
            return _repository.FetchAllUser();
        }

        public async Task<UserData> CreateUser(UserWithPassword user)
        {
            try
            {

                Console.WriteLine("new userId: " + user.UserData.Id);

                var newUser = _repository.InsertUser(user.UserData);

                return newUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                throw;
            }
        }

        public UserData EditUser(UserData user)
        {
            try
            {
                var updatedUser = _repository.UpdateUser(user);
                return updatedUser;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                throw;
            }
        }

        public UserData RemoveUser(string id)
        {
            try
            {
                return _repository.DeleteUser(id);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

            }
}
