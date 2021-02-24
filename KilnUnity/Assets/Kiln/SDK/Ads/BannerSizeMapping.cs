using UnityEngine;

namespace Kiln 
{
    public static class BannerSizeMapping
    {
        public static float Width(this BannerSize adSize)
        {
            switch (adSize) {
                case BannerSize.Width300Height50:
                case BannerSize.Width300Height250:
                    return 300;
                case BannerSize.Width320Height50:
                    return 320;
                case BannerSize.Width336Height280:
                    return 336;
                case BannerSize.Width468Height60:
                    return 468;
                case BannerSize.Width728Height90:
                    return 728;
                case BannerSize.Width970Height90:
                case BannerSize.Width970Height250:
                    return 970;
                case BannerSize.ScreenWidthHeight50:
                case BannerSize.ScreenWidthHeight90:
                case BannerSize.ScreenWidthHeight250:
                case BannerSize.ScreenWidthHeight280:
                    var pixels = Screen.width;
                    var dpi = Screen.dpi;
                    var dips = pixels / (dpi / 160.0f);
                    return dips;
                default:

                    // fallback to default size: Width320Height50
                    return 300;
            }
        }


        public static float Height(this BannerSize adSize)
        {
            switch (adSize) {
                case BannerSize.Width300Height50:
                case BannerSize.Width320Height50:
                case BannerSize.ScreenWidthHeight50:
                    return 50;
                case BannerSize.Width468Height60:
                    return 60;
                case BannerSize.Width728Height90:
                case BannerSize.Width970Height90:
                case BannerSize.ScreenWidthHeight90:
                    return 90;
                case BannerSize.Width300Height250:
                case BannerSize.Width970Height250:
                case BannerSize.ScreenWidthHeight250:
                    return 250;
                case BannerSize.Width336Height280:
                case BannerSize.ScreenWidthHeight280:
                    return 280;
                default:
                    // fallback to default size: Width320Height50
                    return 50;
            }
        }
}

}
