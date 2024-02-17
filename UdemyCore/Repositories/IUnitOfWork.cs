namespace UdemyCore.Repositories
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
        void Commit();
    }
}
