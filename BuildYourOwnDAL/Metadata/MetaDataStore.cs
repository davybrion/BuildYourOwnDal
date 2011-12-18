using System;
using System.Collections.Generic;
using System.Reflection;

namespace BuildYourOwnDAL.Metadata
{
    public class MetaDataStore
    {
        private readonly Dictionary<Type, TableInfo> typeToTableInfo = new Dictionary<Type, TableInfo>();

        public TableInfo GetTableInfoFor<TEntity>()
        {
            return GetTableInfoFor(typeof(TEntity));
        }

        public TableInfo GetTableInfoFor(Type entityType)
        {
            if (!typeToTableInfo.ContainsKey(entityType))
            {
                return null;
            }

            return typeToTableInfo[entityType];
        }

        public void BuildMetaDataFor(Assembly assembly)
        {
            BuildMapOfEntityTypesWithTheirTableInfo(assembly);

            foreach (KeyValuePair<Type, TableInfo> pair in typeToTableInfo)
            {
                // we need this info for each entity before we can deal with references to other entities
                LoopThroughPropertiesWith<PrimaryKeyAttribute>(pair.Key, pair.Value, SetPrimaryKeyInfo);
            }

            foreach (KeyValuePair<Type, TableInfo> pair in typeToTableInfo)
            {
                LoopThroughPropertiesWith<ReferenceAttribute>(pair.Key, pair.Value, AddReferenceInfo);
                LoopThroughPropertiesWith<ColumnAttribute>(pair.Key, pair.Value, AddColumnInfo);
            }
        }

        private void BuildMapOfEntityTypesWithTheirTableInfo(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                var typeAttributes = Attribute.GetCustomAttributes(type, typeof(TableAttribute));

                if (typeAttributes.Length > 0)
                {
                    var tableAttribute = (TableAttribute)typeAttributes[0];
                    var tableInfo = new TableInfo(this, tableAttribute.TableName, type);
                    typeToTableInfo.Add(type, tableInfo);
                }
            }
        }

        private void LoopThroughPropertiesWith<TAttribute>(Type entityType, TableInfo tableInfo,
            Action<TableInfo, PropertyInfo, TAttribute> andExecuteFollowingCode)
            where TAttribute : Attribute
        {
            foreach (var propertyInfo in entityType.GetProperties())
            {
                var attribute = GetAttribute<TAttribute>(propertyInfo);

                if (attribute != null)
                {
                    andExecuteFollowingCode(tableInfo, propertyInfo, attribute);
                }
            }
        }

        private void SetPrimaryKeyInfo(TableInfo tableInfo, PropertyInfo propertyInfo, PrimaryKeyAttribute primaryKeyAttribute)
        {
            tableInfo.PrimaryKey = new ColumnInfo(this, primaryKeyAttribute.ColumnName, propertyInfo.PropertyType, propertyInfo);
        }

        private void AddColumnInfo(TableInfo tableInfo, PropertyInfo propertyInfo, ColumnAttribute columnAttribute)
        {
            tableInfo.AddColumn(new ColumnInfo(this, columnAttribute.ColumnName, propertyInfo.PropertyType, propertyInfo));
        }

        private void AddReferenceInfo(TableInfo tableInfo, PropertyInfo propertyInfo, ReferenceAttribute referenceAttribute)
        {
            tableInfo.AddReference(new ReferenceInfo(this, referenceAttribute.ColumnName, propertyInfo.PropertyType, propertyInfo));
        }

        private TAttribute GetAttribute<TAttribute>(PropertyInfo propertyInfo) where TAttribute : Attribute
        {
            var attributes = Attribute.GetCustomAttributes(propertyInfo, typeof(TAttribute));
            if (attributes.Length == 0) return null;
            return (TAttribute)attributes[0];
        }
    }
}