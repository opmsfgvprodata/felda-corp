using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsDapper
{
    public static class DataSetFunction
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> enumerable)
        {
            var dataTable = new DataTable();
            var propertyDescriptorCollection = TypeDescriptor.GetProperties(typeof(T));
            for (int i = 0; i < propertyDescriptorCollection.Count; i++)
            {
                var propertyDescriptor = propertyDescriptorCollection[i];
                var type = propertyDescriptor.PropertyType;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = Nullable.GetUnderlyingType(type);

                dataTable.Columns.Add(propertyDescriptor.Name, type);
            }
            var values = new object[propertyDescriptorCollection.Count];
            foreach (T iListItem in enumerable)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = propertyDescriptorCollection[i].GetValue(iListItem);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
    }
}