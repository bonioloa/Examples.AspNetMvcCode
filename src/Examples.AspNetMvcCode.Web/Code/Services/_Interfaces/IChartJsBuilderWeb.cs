namespace Examples.AspNetMvcCode.Web.Code;

public interface IChartJsBuilderWeb
{
    ChartJsJsonModel BuildReportStatisticsModel(IList<ItemStepStatisticModel> itemStepStatisticList, long processId, DateTime? dateSubmitFrom, DateTime? dateSubmitTo, DateTime? dateExpirationFrom, DateTime? dateExpirationTo);
}