using DotnetAPI.Models;

namespace DotnetAPI.Data;

public interface IUserRepository
{
    public bool SaveChanges();
    public bool AddEntity<T>(T entity);
    // bool UpdateEntity<T>(T entity);
    bool DeleteEntity<T>(T entity);
    DateTime TestConnection();
    User? GetUserById(int userId);
    public IEnumerable<User> GetUsers();
    // User? GetUserByEmail(string email);
    // User? GetUserByUsername(string username);
    // User? GetUserByUsernameAndPassword(string username, string password);
}