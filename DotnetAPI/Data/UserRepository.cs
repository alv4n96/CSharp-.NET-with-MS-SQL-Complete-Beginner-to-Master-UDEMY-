using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.DTOs.User;
using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data;

public class UserRepository : IUserRepository
{
    private readonly List<User> _users;
    private DataContextEF _entityFramework;
    // IMapper _mapper;

    public UserRepository(IConfiguration config)
    {
        _users = new List<User>();
        _entityFramework = new DataContextEF(config);
        // _mapper = new Mapper(new MapperConfiguration(cfg =>
        // {
        //     cfg.CreateMap<CreateUserDTO, User>();
        //     cfg.CreateMap<UpdateUserDTO, User>();
        // }));
    }

    public bool SaveChanges()
    {
        return _entityFramework.SaveChanges() > 0;
    }

    public bool AddEntity<T>(T entity)
    {
        if (entity != null)
        {
            _entityFramework.Add(entity);
            return true;
        }
        return false;
    }

    public IEnumerable<User> GetUsers()
    {
        return _entityFramework.Users.ToList();
    }

    public User? GetUserById(int userId)
    {
        return _entityFramework.Users
              .Where(u => u.UserId == userId)
              .FirstOrDefault();
    }

    public DateTime TestConnection()
    {
        using var command = _entityFramework.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT GETDATE()";
        _entityFramework.Database.OpenConnection();
        var result = command.ExecuteScalar();
        return Convert.ToDateTime(result);
    }

    public bool DeleteEntity<T>(T entity)
    {
        if (entity != null)
        {
            _entityFramework.Remove(entity);
            return true;
        }
        return false;
    }

    // public bool UpdateEntity<T>(T entity)
    // {
    //     if (entity != null)
    //     {
    //         _entityFramework.Update(entity);
    //         return true;
    //     }
    //     return false;
    // }
}