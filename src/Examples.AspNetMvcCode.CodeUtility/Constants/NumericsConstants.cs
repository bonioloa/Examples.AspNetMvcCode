namespace Examples.AspNetMvcCode.CodeUtility
{
    /// <summary>
    /// constants for numerics structures and classes
    /// </summary>
    public static class NumericsConstants
    {
        /// <summary>
        /// Returns the max length available for decimal numbers saved as strings. 
        /// It's not MinValue.Length because the most significant digit reaches 7 (not 9),
        /// so we need to cut one position to be sure to not get nasty overflows
        /// </summary>
        public static readonly int NumericMaxLength = decimal.MinValue.ToString().Length - 1;

        /// <summary>
        /// constant for when IndexOf method did not find any result
        /// </summary>
        public const int IndexOfNotFound = -1;
    }
}