using BPX.DAL.Context;

namespace BPX.DAL.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly BPXDbContext context;

        public BaseRepository(BPXDbContext context)
        {
            this.context = context;
        }

		public void SaveDBChanges()
		{
            context.SaveChanges();
		}
	}
}