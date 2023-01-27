namespace Examples.AspNetMvcCode.Web.Code;

public interface IResultMessageMapperWeb
{
    AppWarningViewModel GetLocalized(OperationResultViewModel inputOperation);
    OperationResultViewModel SetMessageDataForIdentityDisclosureResult(OperationResultViewModel modelMessage);
    OperationResultViewModel SetResultForItemUserMessageSubmit(OperationResultViewModel modelMessage);
    OperationResultViewModel SetSuccessMessageForAdvance();
    OperationResultViewModel SetSuccessMessageForSave();
    OperationResultViewModel SetSuccessMessageForAbort();
    OperationResultViewModel SetSuccessMessageForRollback();
    OperationResultViewModel SetRoleInclusionMessage(OperationResultViewModel modelMessage, IList<OptionViewModel> includedRoles);
    OperationResultViewModel SetRegistrationResultMessage(OperationResultViewModel modelMessage);
    OperationResultViewModel SetValidateRegistrationResultMessage(OperationResultViewModel modelMessage);
}