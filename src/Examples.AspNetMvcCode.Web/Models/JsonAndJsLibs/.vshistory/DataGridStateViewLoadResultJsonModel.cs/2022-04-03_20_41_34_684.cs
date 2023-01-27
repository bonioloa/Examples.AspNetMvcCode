﻿namespace Comunica.ProcessManager.Web.Models;

public class DataGridStateViewLoadResultJsonModel : ISubmitResultJsonModel
{
    DataGridViewStateJsonModel DataGridViewState { get; set; }


    //interface implementation
    public string ResultCode { get; set; }
    public string ErrorMessage { get; set; }
}