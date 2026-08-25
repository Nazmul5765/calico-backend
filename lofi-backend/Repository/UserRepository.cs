using lofi_backend.Data_Models;
using lofi_backend.Database;
using lofi_backend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace lofi_backend.Repository
{
    public interface IUserRepository
    {
        public UserData FetchUser(string username);
        public UserData FetchUserById(string id);
        public UserData InsertUser(UserData user);
        public UserData UpdateUser(UserData user);
        public UserData DeleteUser(string id);
        List<UserData> FetchAllUser();
    }
    public class UserRepository : IUserRepository
    {
        private readonly LoFiDbContext _db;

        public UserRepository(LoFiDbContext dbContext)
        {
            _db = dbContext;
        }

        public List<UserData> FetchAllUser()
        {
            if (_db.Users.ToList().IsNullOrEmpty()) throw new Exception("No users found");
            return _db.Users.ToList();
        }

        public UserData FetchUserById(string id)
        {
            return _db.Users.First(u => u.Id == id) ?? throw new Exception("User not found");
        }

        public UserData FetchUser(string username)
        {
            return _db.Users.ToList().First(u => u.Username == username) ?? throw new Exception("User not found");
        }

        public UserData InsertUser(UserData user)
        {
            if (_db.Users.Contains(user)) throw new Exception("User exists");

            var newUser = _db.Users.Add(user).Entity;
            Console.WriteLine(newUser.Username + " has been saved");
            _db.SaveChanges();

            return newUser;
        }

        public UserData UpdateUser(UserData user)
        {
            if (_db.Users.Contains(user)) throw new Exception("User exists");

            var updatedUser = _db.Users.Update(user).Entity;
            _db.SaveChanges();
            return updatedUser;
        }

        public UserData DeleteUser(string id)
        {
            var deletedUser = _db.Users.First(u => u.Id == id);

            if (deletedUser == null)
                throw new Exception("User does not exist");

            _db.Users.Remove(deletedUser);

            _db.SaveChanges();

            return deletedUser;

        }
    }
}
