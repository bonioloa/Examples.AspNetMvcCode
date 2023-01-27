using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="DataTable"/>
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// wrap the negation of HasRows to make the call more readable
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this DataTable tbl)
        {
            return !tbl.HasRows();
        }

        /// <summary>
        /// check if datatable is not null and also if it has at least one column
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public static bool HasColumns(this DataTable tbl)
        {
            return tbl != null
               && tbl.Columns != null
               && tbl.Columns.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// check if data table is not null and also has at least one row.
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public static bool HasRows(this DataTable tbl)
        {
            return tbl != null
                && tbl.Rows != null
                && tbl.Rows.GetEnumerator().MoveNext();
            //si usa questo comando perché più efficiente di count , 
            //specialmente se ci sono molte righe
        }

        /// <summary>
        /// checks if the two tables have equal:<br/>
        /// same columns names (case sensitive)<br/>
        /// same columns types<br/>
        /// OPTIONAL: when <paramref name="tbNameMustMatch"/> is true; datatable name (case sensitive)<br/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCompare"></param>
        /// <param name="tbNameMustMatch"></param>
        /// <returns></returns>
        public static bool HasSameBasicSchemaAs(this DataTable source, DataTable toCompare, bool tbNameMustMatch = false)
        {
            if (source is null && toCompare is null)
            {
                return true;
            }
            if (source is null || toCompare is null)
            {
                return false;
            }


            if (tbNameMustMatch && source.TableName.Clean() != toCompare.TableName.Clean())//case sensitive!!!
            {
                return false;
            }

            if ((source.Columns is null || source.Columns.Count <= 0)
                && (toCompare.Columns is null || toCompare.Columns.Count <= 0))
            {
                return true;
            }
            if (source.Columns is null || source.Columns.Count <= 0
                || toCompare.Columns is null || toCompare.Columns.Count <= 0)
            {
                return false;
            }

            if (source.Columns.Count != toCompare.Columns.Count)
            {
                return false;
            }


            //containsAll is case sensitive!!!
            //order of columns does not matter
            if (!source.Columns.Cast<DataColumn>()
                              .Select(x => x.ColumnName)
                              .ContainsAll(
                                    toCompare.Columns.Cast<DataColumn>()
                                                     .Select(x => x.ColumnName)
                                           ))
            {
                return false;
            }

            foreach (DataColumn col in source.Columns)
            {
                if (toCompare.Columns[col.ColumnName].DataType != col.DataType)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// must have the same schema as defined in <see cref="HasSameBasicSchemaAs"/>.<br/>
        /// Rows must have the same data (case sensitive in case of strings), row order will not be considered
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCompare"></param>
        /// <param name="tbNameMustMatch"></param>
        /// <returns></returns>
        /// <remarks>duplicate rows in only one table will fail the check</remarks>
        public static bool HasSameData(this DataTable source, DataTable toCompare, bool tbNameMustMatch = false)
        {
            if (!source.HasSameBasicSchemaAs(toCompare, tbNameMustMatch))
            {
                return false;//exit immediately if schema of both do not match
            }
            if (source is null && toCompare is null)
            {
                return true;
            }
            if (source is null || toCompare is null)
            {
                return false;
            }

            //both of them empty -> equal
            if ((source.Rows is null || source.Rows.Count <= 0)
                && (toCompare.Rows is null || toCompare.Rows.Count <= 0))
            {
                return true;
            }

            //only one of them is empty
            if (source.Rows is null || source.Rows.Count <= 0
                || toCompare.Rows is null || toCompare.Rows.Count <= 0)
            {
                return false;
            }

            if (source.Rows.Count != toCompare.Rows.Count)
            {
                return false;
            }

            IEnumerable<DataRow> differences =
                source.AsEnumerable()
                      .Except(toCompare.AsEnumerable(), DataRowComparer.Default);

            return !differences.Any();
        }
    }
}