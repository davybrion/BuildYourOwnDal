using System.Collections.Generic;
using System.Data.SqlClient;
using BuildYourOwnDAL.Metadata;

namespace BuildYourOwnDAL.Actions
{
    public class BulkInsertAction : DatabaseAction
    {
        public BulkInsertAction(SqlConnection connection, SqlTransaction transaction, MetaDataStore metaDataStore,
            EntityHydrater hydrater, SessionLevelCache sessionLevelCache)
            : base(connection, transaction, metaDataStore, hydrater, sessionLevelCache) { }

        public void Insert<TEntity>(IEnumerable<TEntity> entities, int batchSize, int commandTimeOut)
        {
            var tableInfo = MetaDataStore.GetTableInfoFor<TEntity>();
            var insertStatement = tableInfo.GetInsertStatementWithoutReturningTheIdentityValue();

            var sqlCommandSet = new SqlCommandSetWrapper { Connection = Connection, Transaction = Transaction };

            foreach (var entity in entities)
            {
                var currentCommand = CreateCommand();
                currentCommand.CommandText = insertStatement;

                foreach (var parameterInfo in tableInfo.GetParametersForInsert(entity))
                {
                    currentCommand.CreateAndAddInputParameter(parameterInfo.DbType, "@" + parameterInfo.Name, parameterInfo.Value);
                }

                sqlCommandSet.Append(currentCommand);

                if (sqlCommandSet.CommandCount == batchSize)
                {
                    ExecuteCurrentBatch(sqlCommandSet);
                    sqlCommandSet = new SqlCommandSetWrapper { Connection = Connection, Transaction = Transaction };
                }
            }

            if (sqlCommandSet.CommandCount > 0)
            {
                ExecuteCurrentBatch(sqlCommandSet);
            }
        }

        private void ExecuteCurrentBatch(SqlCommandSetWrapper sqlCommandSet)
        {
            try
            {
                sqlCommandSet.ExecuteNonQuery();
            }
            finally
            {
                sqlCommandSet.Dispose();
            }
        }
    }
}