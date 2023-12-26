using Core.Entities;

namespace WebAPI.Data
{
    public interface IUserRepository
    {
        public List<User> GetUsers();
        public bool IsExistUserByEmail(string email);
        public bool Login(string email, string password);
        public List<OperationClaim> OperationClaims(User user);
    }
}
