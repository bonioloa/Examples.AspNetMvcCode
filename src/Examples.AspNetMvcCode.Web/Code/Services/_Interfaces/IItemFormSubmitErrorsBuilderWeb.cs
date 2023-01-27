namespace Examples.AspNetMvcCode.Web.Code;

public interface IItemFormSubmitErrorsBuilderWeb
{
    string BuildErrorMessage(HashSet<ItemFormSubmitErrorModel> itemFormSubmitErrorSet);
}