using System.Web.Mvc;
using ImageColorDetectMvc.Helpers;

namespace ImageColorDetectMvc.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private static string url = null;

        public ActionResult Index(string imgUrl)
        {
            url = !string.IsNullOrEmpty(imgUrl) ? imgUrl : null;

            return View();
        }

        public ActionResult GetImage()
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            return File(ImageHelper.GetResultImg(url), "image/jpeg");
        }

    }
}
