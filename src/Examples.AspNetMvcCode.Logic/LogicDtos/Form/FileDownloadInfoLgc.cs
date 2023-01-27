namespace Examples.AspNetMvcCode.Logic;

public record FileDownloadInfoLgc(
    byte[] FileContents
    , string ContentType
    , string FileName
    );