namespace Comunica.ProcessManager.Web.Models;

/// <summary>
/// this represents a physical row in a report area
/// if a item have multiple linked items, to represent them correctly
/// we need to leave under the item enough empty rows (or repeat master data) to allow
/// linked items to be displayed together
/// 
/// |item1|linkeditem1toitem1|
/// |     |linkeditem2toitem1|
/// |item2|                  | <- no linked items
/// |item3|linkeditem3toitem3|
/// |item4|linkeditem4toitem4|
/// |     |linkeditem5toitem4|
/// |     |linkeditem6toitem4|
/// 
/// </summary>
public class ReportPositionRowItemModel
{
    public int RowIndex { get; set; }
    public long ItemIdMaster { get; set; }
    public long ProcessIdMaster { get; set; }
    public long ItemIdLinked { get; set; }
    public long ProcessIdLinked { get; set; }
}
