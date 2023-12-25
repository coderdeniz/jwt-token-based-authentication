using Core.Entities;

namespace WebAPI.Data
{
    public interface IUserRepository
    {
        public List<User> GetUsers();
    }
}
