using BuildYourOwnDAL.Metadata;
using Castle.DynamicProxy;

namespace BuildYourOwnDAL
{
    public class LazyLoadingInterceptor : IInterceptor
    {
        private TableInfo tableInfo;
        private readonly Session session;
        private bool needsToBeInitialized = true;

        public LazyLoadingInterceptor(TableInfo tableInfo, Session session)
        {
            this.tableInfo = tableInfo;
            this.session = session;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.Equals("get_" + tableInfo.PrimaryKey.PropertyInfo.Name) ||
                invocation.Method.Name.Equals("set_" + tableInfo.PrimaryKey.PropertyInfo.Name))
            {
                invocation.Proceed();
                return;
            }

            if (needsToBeInitialized)
            {
                needsToBeInitialized = false;
                session.InitializeProxy(invocation.Proxy, invocation.TargetType);
            }

            invocation.Proceed();
        }
    }
}