namespace Examples.AspNetMvcCode.Web.Models;

public record InfoAndLogoViewModel(
    string LogoPath
    , string LogoAltText
    , string AdditionalLogoPath
    , string AdditionalLogoPathText
    , string WebsiteLink
    , string WebsiteDisplayText
    , string Email
    , string EmailDisplayText
    );