using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SMS.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(users u)
        {
            if(ModelState.IsValid)
            {
                users us = new users();
                usersMain main = new usersMain();

                us = main.GetUserDetails(u);

                if (us != null)
                {
                    Session["loginUserId"] = us.user_id.ToString();
                    Session["profilepicture"] = us.user_id.ToString() + ".jpg";
                    Session["loginUserName"] = us.username.ToString();
                    Session["loginUserFullName"] = us.first_name.ToString() +' ' + us.last_name.ToString();
                    FormsAuthentication.SetAuthCookie(us.username,false);
                    return RedirectToAction("Dashboard", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The username or password in incorrect");
                }
            }
           
            
            return View(u);
        }

        public ActionResult Logout()
        {
            Session.RemoveAll();
            FormsAuthentication.SignOut();

            return RedirectToAction("Login");
        }
    }
}