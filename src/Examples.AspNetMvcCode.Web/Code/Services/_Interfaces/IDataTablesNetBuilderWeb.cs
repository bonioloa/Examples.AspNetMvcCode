namespace Examples.AspNetMvcCode.Web.Code;

public interface IDataTablesNetBuilderWeb
{
    DataTablesNetViewModel BuildItemSearchResultModel(IList<ItemAggregatedInfoModel> itemFoundList, bool hasItemEndEditableField);
    DataTablesNetViewModel BuildSupervisorSearchResultModel(IEnumerable<UserFoundModel> usersFound);
}