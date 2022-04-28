using BPX.DAL.Context;

namespace BPX.DAL.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly EFContext efContext;
        protected readonly DPContext dpContext;

        public BaseRepository(EFContext efContext, DPContext dpContext)
        {
            this.efContext = efContext;
            this.dpContext = dpContext;
        }

        public void SaveDBChanges()
        {
            efContext.SaveChanges();
        }
    }
}