using System.Data.SqlClient;
using BuildYourOwnDAL.Metadata;

namespace BuildYourOwnDAL.Actions
{
    public abstract class DatabaseAction
    {
        protected SqlConnection Connection { get; private set; }
        protected SqlTransaction Transaction { get; private set; }
        protected MetaDataStore MetaDataStore { get; private set; }
        protected EntityHydrater Hydrater { get; private set; }
        protected SessionLevelCache SessionLevelCache { get; private set; }

        protected DatabaseAction(SqlConnection connection, SqlTransaction transaction, MetaDataStore metaDataStore,
                                 EntityHydrater hydrater, SessionLevelCache sessionLevelCache)
        {
            Connection = connection;
            Transaction = transaction;
            MetaDataStore = metaDataStore;
            Hydrater = hydrater;
            SessionLevelCache = sessionLevelCache;
        }

        protected SqlCommand CreateCommand()
        {
            var command = Connection.CreateCommand();
            command.Transaction = Transaction;
            return command;
        }
    }
}