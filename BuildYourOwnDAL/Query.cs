using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BuildYourOwnDAL.Actions;
using BuildYourOwnDAL.Metadata;

namespace BuildYourOwnDAL
{
    public interface IQuery
    {
        void AddParameter(string name, object value, DbType dbType);
        TResult GetSingleResult<TResult>();
        IEnumerable<TResult> GetResults<TResult>();
        int ExecuteNonQuery();
    }

    public class Query : IQuery
    {
        private readonly SqlCommand command;
        private readonly MetaDataStore metaDataStore;
        private readonly EntityHydrater hydrater;

        public Query(SqlCommand command, MetaDataStore metaDataStore, EntityHydrater hydrater)
        {
            this.command = command;
            this.metaDataStore = metaDataStore;
            this.hydrater = hydrater;
        }

        public void AddParameter(string name, object value, DbType dbType)
        {
            command.CreateAndAddInputParameter(dbType, name, value);
        }

        public TResult GetSingleResult<TResult>()
        {
            var tableInfo = metaDataStore.GetTableInfoFor<TResult>();

            if (tableInfo == null)
            {
                var scalar = (TResult)command.ExecuteScalar();
                command.Dispose();
                return scalar;
            }

            var result = hydrater.HydrateEntity<TResult>(command);
            command.Dispose();
            return result;
        }

        public IEnumerable<TResult> GetResults<TResult>()
        {
            var tableInfo = metaDataStore.GetTableInfoFor<TResult>();

            if (tableInfo == null)
            {
                var listOfValues = GetListOfValues<TResult>();
                command.Dispose();
                return listOfValues;
            }

            var result = hydrater.HydrateEntities<TResult>(command);
            command.Dispose();
            return result;
        }

        private IEnumerable<TResult> GetListOfValues<TResult>()
        {
            using (var reader = command.ExecuteReader())
            {
                var list = new List<object>();
                while (reader.Read())
                {
                    list.Add(reader.GetValue(0));
                }
                return list.Cast<TResult>();
            }
        }

        public int ExecuteNonQuery()
        {
            var rowsAffected = command.ExecuteNonQuery();
            command.Dispose();
            return rowsAffected;
        }
    }
}