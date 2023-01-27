using System;

namespace Examples.AspNetMvcCode.CodeUtility.Extensions
{
    /// <summary>
    /// Custom extensions for Generic <see cref="Enum"/> types
    /// </summary>
    public static class GenericEnumExtensions
    {
        /// <summary>
        /// Convert a generic enum TY to another generic enum TX. 
        /// MANDATORY CONDITION: both types must have the same flag names and values.
        /// </summary>
        /// <typeparam name="TX">output enum type</typeparam>
        /// <typeparam name="TY">input enum type</typeparam>
        /// <param name="source">input enum param</param>
        /// <returns></returns>
        public static TX ToEnumType<TX, TY>(this TY source)
            where TX : struct, IConvertible
            where TY : struct, IConvertible
        {
            if (!typeof(TX).IsEnum)
            {
                string tmpError = $"{nameof(ToEnumType)} - output type is not a enum";
                //Log.Logger.Error(tmpError);
                throw new InvalidOperationException(tmpError);
            }
            if (!typeof(TY).IsEnum)
            {
                string tmpError = $"{nameof(ToEnumType)} - input type is not a enum";
                //Log.Logger.Error(tmpError);
                throw new InvalidOperationException(tmpError);
            }

            return source.ToString().ToEnum<TX>();
        }
    }
}