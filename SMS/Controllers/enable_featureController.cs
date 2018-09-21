using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class enable_featureController : Controller
    {
        [HttpGet]
        public ActionResult AllUserList()
        {
            enable_featuresMain main = new enable_featuresMain();

            return View(main.AllUserList());
        }
        [HttpGet]
        public ActionResult EditFeature(int user_id)
        {
            enable_featuresMain main = new enable_featuresMain();

            return View(main.AllFeatureList(user_id));
        }

        [HttpPost]
        public ActionResult EditFeature(List<enable_features> feature)
        {
            enable_featuresMain mstMain = new enable_featuresMain();

            foreach (var feat in feature)
            {
                if(feat.active)
                    mstMain.AddFeature(feat);
                else
                    mstMain.DeleteFeature(feat);
            }

           

            return RedirectToAction("AllUserList");
        }

    }
}