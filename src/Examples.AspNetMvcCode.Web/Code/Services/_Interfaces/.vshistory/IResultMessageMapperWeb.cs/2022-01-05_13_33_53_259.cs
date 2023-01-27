namespace Comunica.ProcessManager.Web.Code;

public interface IResultMessageMapperWeb
{
    AppWarningViewModel GetLocalized(OperationResultViewModel inputOperation);
    string GetMessageForSubmitFormErrors(IList<MessageField> fieldsToWarn);
    OperationResultViewModel SetMessageDataForIdentityDisclosureResult(OperationResultViewModel modelMessage);
    OperationResultViewModel SetResultForItemUserMessageSubmit(OperationResultViewModel modelMessage);
    OperationResultViewModel SetSuccessMessageForAdvance(OperationResultViewModel modelMessage);
    OperationResultViewModel SetSuccessMessageForSave(OperationResultViewModel modelMessage);
    OperationResultViewModel SetSuccessMessageForAbort(OperationResultViewModel modelMessage);
    OperationResultViewModel SetSuccessMessageForRollback(OperationResultViewModel modelMessage);
    OperationResultViewModel SetRoleInclusionMessage(OperationResultViewModel modelMessage, IList<OptionViewModel> includedRoles);
    OperationResultViewModel SetRegistrationResultMessage(OperationResultViewModel modelMessage);
    OperationResultViewModel SetValidateRegistrationResultMessage(OperationResultViewModel modelMessage);
}
