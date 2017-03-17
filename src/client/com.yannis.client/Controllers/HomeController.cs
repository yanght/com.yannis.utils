using com.yannis.utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace com.yannis.client.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult VerfyCode()
        {
            string code = string.Empty;
            var bmpBytes = VerifyCodeHelper.VerifyCode(4, 100, 50, out code);
            return File(bmpBytes, @"image/jpeg");
        }

    }
}