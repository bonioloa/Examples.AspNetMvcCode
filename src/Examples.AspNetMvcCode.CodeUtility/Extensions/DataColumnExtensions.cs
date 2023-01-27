using System.Data;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for <see cref="DataColumn"/>
    /// </summary>
    public static class DataColumnExtensions
    {
        /// <summary>
        /// checks if DataColumn has <see cref="DataColumn.ExtendedProperties"/>
        /// </summary>
        /// <param name="dataColumn"></param>
        /// <returns></returns>
        /// <remarks>null safe. returns false if <paramref name="dataColumn"/> or <see cref="DataColumn.ExtendedProperties"/> are null or empty </remarks>
        public static bool HasExtendedProperties(this DataColumn dataColumn)
        {
            return
                dataColumn != null
                && dataColumn.ExtendedProperties != null
                && dataColumn.ExtendedProperties.GetEnumerator().MoveNext();
        }
    }
}