using System;
using System.Data.SqlClient;
using BuildYourOwnDAL.Metadata;

namespace BuildYourOwnDAL.Actions
{
    public class InitializeProxyAction : DatabaseAction
    {
        public InitializeProxyAction(SqlConnection connection, SqlTransaction transaction, MetaDataStore metaDataStore,
            EntityHydrater hydrater, SessionLevelCache sessionLevelCache)
            : base(connection, transaction, metaDataStore, hydrater, sessionLevelCache)
        {
        }

        public void InitializeProxy(object proxy, Type targetType)
        {
            using (var command = CreateCommand())
            {
                var tableInfo = MetaDataStore.GetTableInfoFor(targetType);
                var query = tableInfo.GetSelectStatementForAllFields();
                tableInfo.AddWhereByIdClause(query);

                object id = tableInfo.PrimaryKey.PropertyInfo.GetValue(proxy, null);
                command.CommandText = query.ToString();
                command.CreateAndAddInputParameter(tableInfo.PrimaryKey.DbType, tableInfo.GetPrimaryKeyParameterName(), id);

                Hydrater.UpdateEntity(targetType, proxy, command);
            }
        }
    }
}