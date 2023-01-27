namespace Comunica.ProcessManager.Web.Models;

public class ItemFormStepperSubmitResultViewModel
{
    public bool DisplayTitleTag { get; set; }
    public IHtmlContent Title { get; set; }

    public bool DisplayFirstMessageTag { get; set; }
    public IHtmlContent FirstMessage { get; set; }

    public string ItemCodeStyle { get; set; }

    public bool DisplayLastMessageTag { get; set; }
    public IHtmlContent LastMessage { get; set; }

    public bool DisplayGreetingsTag { get; set; }
    public IHtmlContent Greetings { get; set; }
}
