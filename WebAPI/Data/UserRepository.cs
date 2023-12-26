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

        public List<OperationClaim> OperationClaims(User user)
        {
            var result = from operationClaim in _context.OperationClaims
                         join userOperationClaim in _context.UserOperationClaims
                             on operationClaim.RecordId equals userOperationClaim.OperationClaimId
                         where userOperationClaim.UserId == user.RecordId
                         select new OperationClaim { RecordId = operationClaim.RecordId, Name = operationClaim.Name };

            return result.ToList();
        }
    }
}
