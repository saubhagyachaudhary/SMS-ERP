using SMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class ProfileController : BaseController
    {
        // GET: Profile
        public ActionResult ProfileSetting()
        {
            users u = new users();
            usersMain main = new usersMain();

            u = main.GetUserProfileDetails(Request.Cookies["loginUserName"].Value.ToString());
           //u.password = null;
            return View(u);
        }
        [HttpPost]
        public ActionResult ProfileSetting(users user)
        {
            string fileName = Request.Cookies["loginUserId"].Value.ToString() + Path.GetExtension(user.profilePicture.FileName);
            fileName = Path.Combine(Server.MapPath("~/images/users/"),fileName);
            user.profilePicture.SaveAs(fileName);
           
            return RedirectToAction("Dashboard","Home") ;
        }
    }
}