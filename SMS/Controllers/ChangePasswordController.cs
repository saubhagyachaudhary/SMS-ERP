using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class ChangePasswordController : Controller
    {
        [HttpGet]
        public ActionResult Change_Password()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Change_Password(change_password password)
        {
            if(ModelState.IsValid)
            {
                if(password.new_password != password.confirm_password)
                {
                    ModelState.AddModelError(String.Empty, "New password and confirm password does not matched.");
                    return View();
                }

                change_passwordMain main = new change_passwordMain(); 

                password.user_id = int.Parse(Request.Cookies["loginUserId"].Value);

                password.username = Request.Cookies["loginUserName"].Value.ToString();

                if(main.checkPassword(password) == 0)
                {
                    ModelState.AddModelError(String.Empty, "Old password is incorrect.");
                    return View();
                }

                main.changePassword(password);

                return View("success");
            }
            return View();
        }
    }
}