using Dapper.FastCrud;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace HeadRepositoryNet.Helpers
{
    public class QueryHelper<T>
    {
        public static string BuildSelectQuery(Dictionary<string, string> queryParams)
        {

            string selectQ  = "select " + OrmConfiguration.GetSqlBuilder<T>().ConstructColumnEnumerationForSelect()
                + " from " + OrmConfiguration.GetSqlBuilder<T>().GetTableName();

            return selectQ + QueryOptions(queryParams);

        }

        public static string QueryOptions(Dictionary<string, string> queryParams)
        {
            string limit = "", offset = "", orderBy = "";

            foreach (var dic in queryParams)
            {
                string value = dic.Value;
                string key = dic.Key;
                string valUpper;
                switch (dic.Key)
                {
                    case "limit":
                        limit = ConcatIfNumber(key, value);
                        break;

                    case "offset":
                        offset = ConcatIfNumber(key, value);
                        break;

                    case "orderBy":
                        if (ExistProperty(value, out valUpper))
                            orderBy = " order by \"" + valUpper + "\"";
                        break;

                    case "orderByDesc":
                        if (ExistProperty(value, out valUpper))
                            orderBy = " order by \"" + valUpper + "\" desc";
                        break;
                }

            }
            return orderBy + limit + offset;
        }

        private static string ConcatIfNumber(
            string thirst, 
            string second, 
            string delimiter = " ")
        {
            int number;
            
            bool result = Int32.TryParse(second, out number);
            if (result)
            {
                return " " + thirst + delimiter + second;
            }
            return "";

        }

        private static bool ExistProperty(string propertyName, out string valUpper)
        {
            var sb = new StringBuilder(propertyName);
            sb[0] = char.ToUpper(sb[0]);
            valUpper = sb.ToString();

            // Get the Type object corresponding to MyClass.
            Type entityType = typeof(T);
            // Get the PropertyInfo object by passing the property name.
            PropertyInfo propInfo = entityType.GetProperty(valUpper);
            return propInfo != null;
        }
    }
}
