using System;

namespace BuildYourOwnDAL.Metadata
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class TableAttribute : Attribute
    {
        public string TableName { get; private set; }

        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class PrimaryKeyAttribute : Attribute
    {
        public string ColumnName { get; private set; }

        public PrimaryKeyAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; private set; }

        public ColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ReferenceAttribute : Attribute
    {
        public string ColumnName { get; private set; }

        public ReferenceAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}