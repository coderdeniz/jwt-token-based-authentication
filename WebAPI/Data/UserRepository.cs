using Core.Entities;

namespace WebAPI.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        public bool IsExistUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(u => u.Email == email) != null ? true : false;
        }

        public bool Login(string email, string password)
        {
            return _context.Users.SingleOrDefault(u => u.Email == email && u.Password == password) != null ? true : false;
        }
    }
}
