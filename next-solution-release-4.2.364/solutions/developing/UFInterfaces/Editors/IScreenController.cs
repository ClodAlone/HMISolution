using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
#if !WINDOWS_UWP
using System.Windows.Media;
#else
using Windows.UI;
#endif
using UFInterfaces.Converters;

namespace DocumentManager.ComponentService
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum TileSize
    {
        ExtraSmall,
        Small,
        Large,
        ExtraLarge,
        Live
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum BingMapKind
    {
        Road,
        Area,
        Hybrid
    }

    public interface IScreenController
    {
        bool EnablePageChangeGesture { get;}
        String GetTitle();
        StartType GetStartType();
        [Obsolete]
        TouchType GetTouchType();
        ThemeType GetTheme();

        List<IScreenController> GetScreenControllers();
        List<IScreenController> GetScreenControllers(IDocumentManager idoc);
        List<Uri> GetScreenLists();
        List<Uri> GetScreenLists(IDocumentManager idoc);
        Uri GetStartupScreen();
        Uri GetTopScreen();
        Uri GetLeftScreen();
        Uri GetRightScreen();
        Uri GetBottomScreen();
        bool GetLayoutScreenOrder();

        List<Uri> GetGadgetScreens();
        List<Uri> GetAutoLoadScreens();
        Uri GetAppBarTopScreen();
        Uri GetAppBarBottomScreen();
        Uri GetAppBarLeftScreen();
        Uri GetAppBarRightScreen();
        String GetProjectCulture();
        String GetProjectConverter();

        bool GetHasSpeechEnabled();
        double GetSpeechConfidenceLevel();
        String GetSpeechCulture();
        List<String> GetDefaultSpeechCommands();

        bool GetHasGeoCoordinates();
        double GetLatitude();
        double GetLongitude();
        Color GetIdentityColor();
        bool IsVisible();
        TileSize TileSize();
        String GetDescription();
        String GetIconSource();
        double GetMapMinZoomLevelVisibility();
        double GetMapMaxZoomLevelVisibility();

        String GetUsersVisibility();
        String GetRolesVisibility();

        Color GetIdentityColor(Uri uri);
        bool IsVisible(Uri uri);
        TileSize TileSize(Uri uri);
        String GetDescription(Uri uri);
        bool GetHasGeoCoordinates(Uri uri);
        double GetLatitude(Uri uri);
        double GetLongitude(Uri uri);
        String GetLatitudeTag(Uri uri);
        String GetLongitudeTag(Uri uri);
        double GetMapMinZoomLevelVisibility(Uri uri);
        double GetMapMaxZoomLevelVisibility(Uri uri);

        String GetUsersVisibility(Uri uri);
        String GetRolesVisibility(Uri uri);

        BingMapKind GetBingMapKind();
    }
}
