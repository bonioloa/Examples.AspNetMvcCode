namespace Examples.AspNetMvcCode.Data;
/// <summary>
/// handles IHtmlContent params in both direction.<br/>
/// Must be included in SqlMapper before Dapper calling methods. 
/// Check in <see cref="DataCommandManagerTenant.Query{T}(CommandExecutionDb)"/>
/// </summary>
internal class IHtmlContentTypeHandler : SqlMapper.TypeHandler<IHtmlContent>
{
    public override void SetValue(
        IDbDataParameter parameter
        , IHtmlContent value
        )
    {
        parameter.DbType = DbType.String;
        parameter.Value = value?.GetStringContent();
    }
    public override IHtmlContent Parse(object value)
    {
        return new HtmlString(value?.ToString());
    }
}