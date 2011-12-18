using System.Data.SqlClient;
using BuildYourOwnDAL.Metadata;

namespace BuildYourOwnDAL.Actions
{
    public class GetByIdAction : DatabaseAction
    {
        public GetByIdAction(SqlConnection connection, SqlTransaction transaction, MetaDataStore metaDataStore,
            EntityHydrater hydrater, SessionLevelCache sessionLevelCache)
            : base(connection, transaction, metaDataStore, hydrater, sessionLevelCache)
        {
        }

        public TEntity Get<TEntity>(object id)
        {
            var cachedEntity = SessionLevelCache.TryToFind(typeof(TEntity), id);
            if (cachedEntity != null) return (TEntity)cachedEntity;

            using (var command = CreateCommand())
            {
                var tableInfo = MetaDataStore.GetTableInfoFor<TEntity>();

                var query = tableInfo.GetSelectStatementForAllFields();
                tableInfo.AddWhereByIdClause(query);

                command.CommandText = query.ToString();
                command.CreateAndAddInputParameter(tableInfo.PrimaryKey.DbType, tableInfo.GetPrimaryKeyParameterName(), id);
                return Hydrater.HydrateEntity<TEntity>(command);
            }
        }
    }
}