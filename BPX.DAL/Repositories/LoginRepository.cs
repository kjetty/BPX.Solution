using BPX.DAL.Context;
using BPX.Domain.DbModels;
using BPX.Utils;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using X.PagedList;

namespace BPX.DAL.Repositories
{
	public class LoginRepository : BaseRepository, ILoginRepository
    {
        public LoginRepository(EFContext efContext, DPContext dpContext) : base(efContext, dpContext)
        {
        }

        public IPagedList<Login> GetPaginatedRecords(int pageNumber, int pageSize, string statusFlag, string sortByColumn, string sortOrder, string searchForString, string filterJson)
        {
            throw new NotImplementedException();
        }

        public Login GetRecordById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Login> GetRecordsByFilter(Expression<Func<Login, bool>> filter)
        {
            return efContext.Logins.Where(filter);
        }

        public void InsertRecord(Login entity)
        {
            efContext.Logins.Add(entity);
        }

        public void UpdateRecord(Login entity)
        {
            efContext.Entry(entity).State = EntityState.Modified;
        }

        public Login GetLoginByToken(string lToken)
        {
            string dynquery = string.Empty;
            DynamicParameters dynParams = new();

            dynquery += "select * from Logins where upper(StatusFlag) = @StatusFlag and LToken = @LToken";
            dynParams.Add("StatusFlag", RecordStatus.Active.ToUpper());
            dynParams.Add("LToken", lToken);

            using IDbConnection connection = dpContext.CreateConnection();
            Login login = connection.QuerySingleOrDefault<Login>(dynquery, dynParams);

            return login;
        }
    }

    public interface ILoginRepository : IRepository<Login>
    {
        Login GetLoginByToken(string lToken);
    }
}