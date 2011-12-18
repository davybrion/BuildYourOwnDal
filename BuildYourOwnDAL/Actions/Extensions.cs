using System;
using System.Data;
using System.Data.SqlClient;

namespace BuildYourOwnDAL.Actions
{
    public static class Extensions
    {
        public static void CreateAndAddInputParameter(this SqlCommand command, DbType type, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.Direction = ParameterDirection.Input;
            parameter.DbType = type;
            parameter.ParameterName = name;

            if (value == null)
            {
                parameter.IsNullable = true;
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = value;
            }

            command.Parameters.Add(parameter);
        }        
    }
}