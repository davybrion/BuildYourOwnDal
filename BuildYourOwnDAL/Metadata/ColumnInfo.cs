using System;
using System.Data;
using System.Reflection;

namespace BuildYourOwnDAL.Metadata
{
    public class ColumnInfo : MetaData
    {
        public string Name { get; private set; }
        public Type DotNetType { get; private set; }
        public DbType DbType { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }

        public ColumnInfo(MetaDataStore store, string name, Type dotNetType, PropertyInfo propertyInfo)
            : this(store, name, dotNetType, TypeConverter.ToDbType(dotNetType), propertyInfo)
        {
        }

        public ColumnInfo(MetaDataStore store, string name, Type dotNetType, DbType dbType, PropertyInfo propertyInfo)
            : base(store)
        {
            Name = name;
            DotNetType = dotNetType;
            DbType = dbType;
            PropertyInfo = propertyInfo;
        }
    }
}