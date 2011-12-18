using System.Reflection;
using BuildYourOwnDAL.Metadata;

namespace BuildYourOwnDAL
{
    public interface ISessionFactory
    {
        ISession CreateSession();
    }

    public class SessionFactory : ISessionFactory
    {
        private string connectionString;
        private MetaDataStore metaDataStore;

        public static ISessionFactory Create(Assembly assembly, string connectionString)
        {
            var sessionFactory = new SessionFactory { connectionString = connectionString, metaDataStore = new MetaDataStore() };
            sessionFactory.metaDataStore.BuildMetaDataFor(assembly);
            return sessionFactory;
        }

        private SessionFactory() { }

        public ISession CreateSession()
        {
            return new Session(connectionString, metaDataStore);
        }
    }
}