using System;
using System.Web.Mvc;

namespace ImageColorDetectMvc
{
    public class ImageColorDetectAreaRegistration : AreaRegistration
    {
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ImageColorDetect_Default",
                "color-detect/{controller}/{action}/",
                new { controller = "Home", action = "Index" }
            );
        }

        public override string AreaName
        {
            get { return "ImageColorDetect"; }
        }
    }
}