using System.Collections.Generic;
using System.Data.SqlClient;
using BuildYourOwnDAL.Metadata;

namespace BuildYourOwnDAL.Actions
{
    public class FindAllAction : DatabaseAction
    {
        public FindAllAction(SqlConnection connection, SqlTransaction transaction, MetaDataStore metaDataStore,
            EntityHydrater hydrater, SessionLevelCache sessionLevelCache)
            : base(connection, transaction, metaDataStore, hydrater, sessionLevelCache)
        {
        }

        public IEnumerable<TEntity> FindAll<TEntity>()
        {
            using (var command = CreateCommand())
            {
                command.CommandText = MetaDataStore.GetTableInfoFor<TEntity>().GetSelectStatementForAllFields().ToString();
                return Hydrater.HydrateEntities<TEntity>(command);
            }
        }
    }
}