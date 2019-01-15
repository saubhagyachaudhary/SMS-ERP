using SMS.Models;
using SMS.report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace SMS.Controllers
{
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        //string smsurl = ConfigurationManager.AppSettings["SMSGatewayPostURL"].ToString();
        // GET: Login
        public ActionResult Login(string ReturnUrl)
        {
            
            users u = new users();
            u.ReturnUrl = ReturnUrl;

            SMSMessage sms = new SMSMessage();

            return View(u);
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



                    FormsAuthentication.SetAuthCookie(us.username,false);
                  

                    CreateCookie(us,main);

                    string decodedUrl = "";
                    if (!string.IsNullOrEmpty(u.ReturnUrl))
                        decodedUrl = Server.UrlDecode(u.ReturnUrl);

                    if (Url.IsLocalUrl(decodedUrl))
                    {
                        return Redirect(decodedUrl);
                    }
                    else
                    {
                       

                        return RedirectToAction("Dashboard", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "The username or password in incorrect");
                }
            }
           
            
            return View(u);
        }

       
       

        private void CreateCookie(users us, usersMain main)
        {

            HttpCookie StudentCookies = new HttpCookie("features");
            StudentCookies.Value = main.GetUserFeatures(us.user_id);
            Response.Cookies.Add(StudentCookies);

            HttpCookie wedget = new HttpCookie("wedget");
            wedget.Value = main.GetUserWedget(us.user_id);
            Response.Cookies.Add(wedget);

            HttpCookie loginUserId = new HttpCookie("loginUserId");
            loginUserId.Value = us.user_id.ToString();
            Response.Cookies.Add(loginUserId);

            HttpCookie profilepicture = new HttpCookie("profilepicture");
            profilepicture.Value = us.user_id.ToString() + ".jpg";
            Response.Cookies.Add(profilepicture);

            HttpCookie loginUserName = new HttpCookie("loginUserName");
            loginUserName.Value = us.username.ToString();
            Response.Cookies.Add(loginUserName);

            HttpCookie loginUserFullName = new HttpCookie("loginUserFullName");
            loginUserFullName.Value = us.FirstName.ToString() + ' ' + us.lastname.ToString();
            Response.Cookies.Add(loginUserFullName);

            HttpCookie roles = new HttpCookie("roles");
            roles.Value = us.roles;
            Response.Cookies.Add(roles);

            
        }

        public ActionResult Logout()
        {
            //Session.RemoveAll();
            FormsAuthentication.SignOut();
            Response.Cookies["features"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["loginUserId"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["profilepicture"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["loginUserName"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["loginUserFullName"].Expires = DateTime.Now.AddDays(-1);
            Response.Cookies["roles"].Expires = DateTime.Now.AddDays(-1);

            return RedirectToAction("Login");
        }
    }
}