using BPX.DAL.Context;

namespace BPX.DAL.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly BPXDbContext _context;

        protected BaseRepository(BPXDbContext context)
        {
            _context = context;
        }
    }
}