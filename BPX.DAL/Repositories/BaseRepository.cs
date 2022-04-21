using BPX.DAL.Context;

namespace BPX.DAL.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly EFContext efContext;

        public BaseRepository(EFContext efContext)
        {
            this.efContext = efContext;
            
        }

		public void SaveDBChanges()
		{
            efContext.SaveChanges();
            //context.ChangeTracker.Clear();
        }
	}
}