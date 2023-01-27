namespace Comunica.ProcessManager.Web.Code;

public class ReportingMapperWeb : IReportingMapperWeb
{
    private readonly ILogger<ReportingMapperWeb> _logger;
    private readonly ContextApp _contextApp;
    private readonly ContextTenant _contextTenant;

    private readonly IExcelBuildNew _fmExcelManager;

    public ReportingMapperWeb(
        ILogger<ReportingMapperWeb> logger
        , ContextApp contextApp
        , ContextTenant contextTenant
        , IExcelBuildNew fmExcelManager
        )
    {
        _logger = logger;
        _contextApp = contextApp;
        _contextTenant = contextTenant;
        _fmExcelManager = fmExcelManager;
    }


    public FileDownloadInfoModel MapFile(
        IList<ReportingAreaModel> dataToLoad
        , ReportConfigFileLgc reportConfigFile
        , DateTime timeStamp
        )
    {
        IList<ExcelSheetFm> sheetList =
            MapSheetList(
                reportConfigFile.Sheets
                , new List<IList<ReportingAreaModel>> { dataToLoad }
                );

        ExcelGenerationFm newExcelFile =
            MapExcelFilePartial(
                reportConfigFile
                , timeStamp
                , sheetList
                );

        FileCompleteFm generatedDocument =
            _fmExcelManager.CreateNewFromData(
                newExcelFile
                );

        _logger.LogAppInformation($"document '{generatedDocument.Filename}' created successfully");

        return new FileDownloadInfoModel()
        {
            FileContents = generatedDocument.FileRawContent,
            ContentType = generatedDocument.ContentType,
            FileName = generatedDocument.Filename,
        };
    }




    private static DocumentFontFm MapFont(ReportConfigFontLgc reportConfigFont)
    {
        return reportConfigFont is null ? new DocumentFontFm() :
            new DocumentFontFm
            {
                FontIdCode = reportConfigFont.FontIdCode,
                FontName = reportConfigFont.FontName,
                FontSize = reportConfigFont.FontSize,
                FontHexColor = reportConfigFont.FontHexColor,
                IsBold = reportConfigFont.IsBold,
                IsItalic = reportConfigFont.IsItalic,
                IsStrikeout = reportConfigFont.IsStrikeout,
                Underline = reportConfigFont.UnderlineType
                                .ToEnumType<FontUnderlineTypeFm, ReportFontUnderlineType>(),
            };
    }


    private static IDictionary<string, DocumentFontFm> MapReportConfigFontDict(
        IDictionary<string, ReportConfigFontLgc> reportConfigFontDict
        )
    {
        IDictionary<string, DocumentFontFm> fontDict =
           new Dictionary<string, DocumentFontFm>();
        if (reportConfigFontDict is null)
        {
            return fontDict;
        }
        ReportConfigFontLgc tmpFont;
        foreach (string styleIdCode in reportConfigFontDict.Keys)
        {
            tmpFont = reportConfigFontDict[styleIdCode];
            fontDict.Add(styleIdCode, MapFont(tmpFont));
        }
        return fontDict;
    }


    private static ExcelCellStyleFm MapStyle(ReportConfigStyleLgc reportConfigStyle)
    {
        return reportConfigStyle is null ? new ExcelCellStyleFm() :
            new ExcelCellStyleFm
            {
                StyleIdCode = reportConfigStyle.StyleIdCode,
                FontIdCode = reportConfigStyle.FontIdCode,

                BackgroundHexColor = reportConfigStyle.BackgroundHexColor,

                VerticalAlignment = reportConfigStyle.VerticalAlignmentType
                                        .ToEnumType<VerticalAlignmentFm, ReportVerticalAlignment>(),
                HorizontalAlignment = reportConfigStyle.HorizontalAlignmentType
                                        .ToEnumType<HorizontalAlignmentFm, ReportHorizontalAlignment>(),

                AllBordersSingleConfig = reportConfigStyle.AllBordersSingleConfig,
                AllBordersStyle = reportConfigStyle.AllBordersStyleType.ToEnumType<BorderStyleFm, ReportBorderStyle>(),
                AllBordersHexColor = reportConfigStyle.AllBordersHexColor,

                BorderStyleTop = reportConfigStyle.BorderStyleTopType.ToEnumType<BorderStyleFm, ReportBorderStyle>(),
                BordersHexColorTop = reportConfigStyle.BordersHexColorTop,

                BorderStyleRight = reportConfigStyle.BorderStyleRightType.ToEnumType<BorderStyleFm, ReportBorderStyle>(),
                BordersHexColorRight = reportConfigStyle.BordersHexColorRight,

                BorderStyleBottom = reportConfigStyle.BorderStyleBottomType.ToEnumType<BorderStyleFm, ReportBorderStyle>(),
                BordersHexColorBottom = reportConfigStyle.BordersHexColorBottom,

                BorderStyleLeft = reportConfigStyle.BorderStyleLeftType.ToEnumType<BorderStyleFm, ReportBorderStyle>(),
                BordersHexColorLeft = reportConfigStyle.BordersHexColorLeft
            };
    }


    private static IDictionary<string, ExcelCellStyleFm> MapReportConfigStyleDict(
        IDictionary<string, ReportConfigStyleLgc> reportConfigStyleDict
        )
    {
        IDictionary<string, ExcelCellStyleFm> cellStyleRepDict =
           new Dictionary<string, ExcelCellStyleFm>();
        if (reportConfigStyleDict is null)
        {
            return cellStyleRepDict;
        }
        ReportConfigStyleLgc tmpStyle;
        foreach (string styleIdCode in reportConfigStyleDict.Keys)
        {
            tmpStyle = reportConfigStyleDict[styleIdCode];
            cellStyleRepDict.Add(styleIdCode, MapStyle(tmpStyle));
        }
        return cellStyleRepDict;
    }


    private IList<ExcelSheetAreaFm> MapSheetAreaList(
        IList<ReportConfigSheetAreaLgc> reportConfigSheetAreaList
        , IList<ReportingAreaModel> sheetAreaList
        )
    {
        IList<ExcelSheetAreaFm> areaList = new List<ExcelSheetAreaFm>();
        ExcelSheetAreaDataFm tmpAreaData;
        ExcelSheetAreaImageFm tmpAreaImage;
        ExcelSheetAreaTextFm tmpAreaText;

        IEnumerable<DataTable> tmpDataFound;

        foreach (ReportConfigSheetAreaLgc sheetArea in reportConfigSheetAreaList)
        {
            tmpDataFound = null;//reset

            if (sheetArea is ReportConfigAreaDataLgc areaData)
            {
                tmpAreaData = new ExcelSheetAreaDataFm()
                {
                    Progressive = areaData.Progressive,
                    OffsetRowsPositionStart = areaData.OffsetRowsPositionStart,
                    OffsetColumnsPositionStart = areaData.OffsetColumnsPositionStart,
                    PositionRelativeToPrevious = areaData.PositionRelativeToPrevious
                        .ToEnumType<AreaRelativePositionFm, ReportAreaRelativePosition>(),

                    DtNameAsHeader = areaData.DtNameAsHeader,
                    DtNameHeaderStyleIdCode = areaData.DtNameHeaderStyleIdCode,
                    DtColumnsHeadersHide = areaData.DtColumnsHeadersHide,
                    DtColumnsHeadersStyleIdCode = areaData.DtColumnsHeadersStyleIdCode,
                    DtRowsStyleIdCode = areaData.DtRowsStyleIdCode,
                };

                tmpDataFound = areaData.DtTypeToLoad switch
                {
                    ReportAreaDataToLoad.DataMainItemsByProcess =>
                        sheetAreaList.Where(sad => sad.Type == areaData.DtTypeToLoad
                                                    && sad.ProcessId == areaData.ProcessId)
                                     .Select(sad => sad.Data),

                    ReportAreaDataToLoad.DataMainLinkedItemsByProcess =>
                            sheetAreaList.Where(sad => sad.Type == areaData.DtTypeToLoad
                                                        && sad.ProcessId == areaData.ProcessId)
                                         .Select(sad => sad.Data),

                    //assumption: steps area are consecutive so once we find the first, we add all of them
                    ReportAreaDataToLoad.DataStepItemsByProcess =>
                        sheetAreaList.Where(sad => sad.Type == areaData.DtTypeToLoad
                                                    && sad.StepIndex == areaData.StepProgressive
                                                    && sad.ProcessId == areaData.ProcessId)
                                     .Select(sad => sad.Data),

                    ReportAreaDataToLoad.DataStepLinkedItemsByProcess =>
                        sheetAreaList.Where(sad => sad.Type == areaData.DtTypeToLoad
                                                    && sad.StepIndex == areaData.StepLinkedProgressive
                                                    && sad.ProcessId == areaData.ProcessId)
                                     .Select(sad => sad.Data),

                    //the other types are included only once, so we don't need a progressive
                    _ => sheetAreaList.Where(sad => sad.Type == areaData.DtTypeToLoad)
                                      .Select(sad => sad.Data),
                };
                ;

                if (tmpDataFound.HasValues())
                {
                    tmpAreaData.DtData = tmpDataFound.First();
                    areaList.Add(tmpAreaData);
                }

                continue;
            }
            if (sheetArea is ReportConfigAreaImageLgc areaImage)
            {
                tmpAreaImage = new ExcelSheetAreaImageFm()
                {
                    Progressive = areaImage.Progressive,
                    OffsetRowsPositionStart = areaImage.OffsetRowsPositionStart,
                    OffsetColumnsPositionStart = areaImage.OffsetColumnsPositionStart,
                    PositionRelativeToPrevious = areaImage.PositionRelativeToPrevious
                        .ToEnumType<AreaRelativePositionFm, ReportAreaRelativePosition>(),

                    ImagePath = GetImagePath(areaImage),
                    ImageMaxWidthPx = areaImage.ImageMaxWidthPx,
                    ImageMaxHeightPx = areaImage.ImageMaxHeightPx,
                    WidthInColumnsUnits = areaImage.WidthInColumnsUnits,
                    HeightInRowsUnits = areaImage.HeightInRowsUnits,
                };
                areaList.Add(tmpAreaImage);
                continue;
            }
            if (sheetArea is ReportConfigAreaTextLgc areaText)
            {
                tmpAreaText = new ExcelSheetAreaTextFm
                {
                    Progressive = areaText.Progressive,
                    OffsetRowsPositionStart = areaText.OffsetRowsPositionStart,
                    OffsetColumnsPositionStart = areaText.OffsetColumnsPositionStart,
                    PositionRelativeToPrevious = areaText.PositionRelativeToPrevious
                        .ToEnumType<AreaRelativePositionFm, ReportAreaRelativePosition>(),

                    WidthInColumnsUnits = areaText.WidthInColumnsUnits,
                    HeightInRowsUnits = areaText.HeightInRowsUnits,
                    Text = areaText.TextLocalized,
                    TextStyleIdCode = areaText.TextStyleIdCode,
                };
                areaList.Add(tmpAreaText);
                continue;
            }
            _logger.LogAppError($"unhandled area type {sheetArea.GetType()}");
            throw new WebAppException();
        }
        return areaList;
    }


    public string GetImagePath(ReportConfigAreaImageLgc areaImage)
    {
        string tmpImagePath;
        if (areaImage.ImagePath.StringHasValue())
        {
            tmpImagePath = areaImage.ImagePath;
        }
        else
        {
            if (areaImage.ImageFromTenantContext)
            {
                tmpImagePath = WebAppConstants.AppPathFileTenantsLogo + _contextTenant.LogoFileName;
            }
            else
            {
                if (areaImage.ImageFromUserContext)
                {
                    //TODO image from user context
                    _logger.LogAppError("implement handling of logo image loaded form user context");
                    throw new NotImplementedException();
                }
                else
                {
                    if (areaImage.ImageFromProcess)
                    {
                        //TODO image from process table
                        _logger.LogAppError("implement handling of logo image loaded form process table");
                        throw new NotImplementedException();
                    }
                    else
                    {
                        _logger.LogAppError($"unhandled case for image path retrieval; progressive '{areaImage.Progressive}'");
                        throw new DataException();
                    }
                }

            }
        }//end of image path handling

        return tmpImagePath;
    }


    private ExcelSheetFm MapSheet(
        ReportConfigSheetLgc reportConfigSheet
        , IList<ReportingAreaModel> sheetAreaList
        )
    {
        return new ExcelSheetFm
        {
            //reportConfigSheet.SheetIdCode//unused
            Progressive = reportConfigSheet.Progressive,
            SheetName = reportConfigSheet.SheetNameLocalized,
            MaxCharactersAllowedForColumnWidth = reportConfigSheet.MaxCharactersAllowedForColumnWidth,
            InterlineRowHeight = reportConfigSheet.InterlineRowHeight,
            SheetAreas = MapSheetAreaList(reportConfigSheet.SheetAreas, sheetAreaList),
        };
    }


    private IList<ExcelSheetFm> MapSheetList(
        IList<ReportConfigSheetLgc> reportConfigSheetList
        , IList<IList<ReportingAreaModel>> dataForAreasBySheet
        )
    {
        IList<ExcelSheetFm> sheets = new List<ExcelSheetFm>();
        if (reportConfigSheetList.IsNullOrEmpty())
        {
            return sheets;
        }
        for (int index = 0; index < reportConfigSheetList.Count; index++)
        {
            sheets.Add(
                MapSheet(
                    reportConfigSheetList[index]
                    , dataForAreasBySheet[index]
                    )
                );
        }
        return sheets;
    }


    private ExcelGenerationFm MapExcelFilePartial(
        ReportConfigFileLgc reportConfigFile
        , DateTime timeStamp
        , IList<ExcelSheetFm> sheetList
        )
    {
        return reportConfigFile is null ? new ExcelGenerationFm() :
            new ExcelGenerationFm
            {
                FileType = reportConfigFile.FileType.ToEnumType<FileTypeGenerationFm, ReportFileType>(),
                FormattingCulture = _contextApp.GetCurrentCulture(),
                Fonts = MapReportConfigFontDict(reportConfigFile.Fonts),
                Styles = MapReportConfigStyleDict(reportConfigFile.Styles),
                FileNameWithoutExtension =
                    reportConfigFile.CompleteNamePattern
                        .Replace(AppConstants.ReportPlhFileNamePatternPrefix, reportConfigFile.FilenamePrefix)
                        .Replace(
                            AppConstants.ReportPlhFileNamePatternTimestamp
                            , timeStamp.ToStringDateTimeInvariantForFileName()
                            )
                        .Replace(AppConstants.ReportPlhFileNamePatternProcessName, string.Empty),//not handled for now
                Sheets = sheetList,
            };
    }
}