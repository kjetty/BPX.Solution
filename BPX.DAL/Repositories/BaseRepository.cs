using BPX.DAL.Context;

namespace BPX.DAL.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly BPXDbContext _context;

        public BaseRepository(BPXDbContext context)
        {
            _context = context;
        }

		public void SaveDBChanges()
		{
			_context.SaveChanges();
		}
	}
}