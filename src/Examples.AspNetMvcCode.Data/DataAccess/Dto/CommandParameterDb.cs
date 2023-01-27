namespace Examples.AspNetMvcCode.Data;

/// <summary>
/// Represents Sql Command Parameter configuration
/// </summary>
public class CommandParameterDb : IEquatable<CommandParameterDb>
{
    /// <summary>
    /// order on which parameters will be added to command
    /// </summary>
    internal int Progressive { get; set; }

    /// <summary>
    /// Parameter name. It's a key and must be unique for the same command batch.
    /// </summary>
    /// <remarks>
    /// It's a key and must be unique for the same command batch.<br/>
    /// Must be prefixed with symbol '@'
    /// </remarks>
    internal string Name { get; set; }

    /// <summary>
    /// If not set will be automatically considered <see cref="ParameterDirection.Input"/><br/>
    /// Set this direction only if parameter is <see cref="ParameterDirection.Output"/> or <see cref="ParameterDirection.Input"/>.
    /// </summary>
    /// <remarks><see cref="ParameterDirection.ReturnValue"/> as Direction is not supported, Call Command As ExecuteScalar</remarks>
    internal ParameterDirection? Direction { get; set; } = ParameterDirection.Input;

    /// <summary>
    /// declaring as nullable, because we don't need to set it explicitly for all object types
    /// </summary>
    internal SqlDbType? Type { get; set; } = null;

    /// <summary>
    /// in cause of using user defined types, here must be provided the SqlTypeName
    /// </summary>
    internal string TypeName { get; set; }

    /// <summary>
    /// boxed value associated to Parameter Name. 
    /// Basic struct type (strings, numerics, DateTime) are handled automatically by framework
    /// </summary>
    internal object Value { get; set; }




    /// <summary>
    /// equality
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(CommandParameterDb other)
    {
        return (this is null && other is null)
                || (this != null && other != null
                        && Name.Clean().EqualsInvariant(other.Name.Clean()));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        if (this is null)
        {
            return -1;
        }
        return Name.Clean().GetHashCode(StringComparison.InvariantCultureIgnoreCase);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as CommandParameterDb);
    }
}