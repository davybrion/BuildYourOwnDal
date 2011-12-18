using System;
using System.Data.SqlClient;
using System.Reflection;

namespace BuildYourOwnDAL.Actions
{
    // as found on: http://richardod.blogspot.com/2009/03/speeding-up-sql-server-inserts-by-using.html
    // Modified by Richard OD to exploit .NET 3.5 08 March 2009
    // Copyright (c) 2005 - 2007 Ayende Rahien (ayende@ayende.com)
    // All rights reserved.
    //
    // Redistribution and use in source and binary forms, with or without modification,
    // are permitted provided that the following conditions are met:
    //
    //     * Redistributions of source code must retain the above copyright notice,
    //     this list of conditions and the following disclaimer.
    //     * Redistributions in binary form must reproduce the above copyright notice,
    //     this list of conditions and the following disclaimer in the documentation
    //     and/or other materials provided with the distribution.
    //     * Neither the name of Ayende Rahien nor the names of its
    //     contributors may be used to endorse or promote products derived from this
    //     software without specific prior written permission.
    //
    // THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
    // ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
    // WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    // DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
    // FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    // DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
    // SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
    // CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    // OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
    // THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
    public class SqlCommandSetWrapper : IDisposable
    {
        private static readonly Type commandSetType;
        private readonly object commandSet;
        private readonly Action<SqlCommand> appenderDel;
        private readonly Action disposeDel;
        private readonly Func<int> executeNonQueryDel;
        private readonly Func<SqlConnection> connectionGetDel;
        private readonly Action<SqlConnection> connectionSetDel;
        private readonly Action<SqlTransaction> transactionSetDel;
 
        private int commandCount;
 
        static SqlCommandSetWrapper()
        {
            Assembly systemData = Assembly.Load("System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            commandSetType = systemData.GetType("System.Data.SqlClient.SqlCommandSet");
        }
 
        public SqlCommandSetWrapper()
        {
            commandSet = Activator.CreateInstance(commandSetType, true);
            appenderDel = (Action<SqlCommand>)Delegate.CreateDelegate(typeof(Action<SqlCommand>), commandSet, "Append");
            disposeDel = (Action)Delegate.CreateDelegate(typeof(Action), commandSet, "Dispose");
            executeNonQueryDel = (Func<int>)Delegate.CreateDelegate(typeof(Func<int>), commandSet, "ExecuteNonQuery");
            connectionGetDel = (Func<SqlConnection>)Delegate.CreateDelegate(typeof(Func<SqlConnection>), commandSet, "get_Connection");
            connectionSetDel = (Action<SqlConnection>)Delegate.CreateDelegate(typeof(Action<SqlConnection>), commandSet, "set_Connection");
            transactionSetDel = (Action<SqlTransaction>)Delegate.CreateDelegate(typeof(Action<SqlTransaction>), commandSet, "set_Transaction");
        }
 
        public void Append(SqlCommand command)
        {
            commandCount++;
            appenderDel.Invoke(command);
        }
 
        public int ExecuteNonQuery()
        {
            return executeNonQueryDel.Invoke();
        }
 
        public SqlConnection Connection
        {
            get
            {
                return connectionGetDel.Invoke();
            }
            set
            {
                connectionSetDel.Invoke(value);
            }
        }
 
        public SqlTransaction Transaction
        {
            set
            {
                transactionSetDel.Invoke(value);
            }
        }
 
        public int CommandCount
        {
            get
            {
                return commandCount;
            }
        }
 
        public void Dispose()
        {
            disposeDel.Invoke();
        }
    }
}
