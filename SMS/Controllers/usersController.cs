using Dapper;
using MySql.Data.MySqlClient;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class usersController : Controller
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult AddUser()
        {
            usersMain user = new usersMain();

            var class_list = user.employees();


            IEnumerable<SelectListItem> list = new SelectList(class_list, "user_id", "id");

            ViewData["user_id"] = list;

            return View();
        }

        [HttpPost]
        public ActionResult AddUser(users mst)
        {
            usersMain users = new usersMain();

            users.addUser(mst);

            return RedirectToAction("AllUsersList");
        }

        [HttpGet]
        public ActionResult DeleteUser(int id)
        {
            usersMain stdMain = new usersMain();

            return View(stdMain.FindUser(id));
        }

        [HttpPost]
        public ActionResult DeleteUser(int id, FormCollection collection)
        {
            try
            {
                usersMain stdMain = new usersMain();

                stdMain.DeleteUser(id);

                return RedirectToAction("AllUsersList");
            }
            catch (Exception ex)
            {
                // ModelState.AddModelError(String.Empty, "Sections are created cannot delete");

                return RedirectToAction("AllUsersList");
            }
        }

        [HttpGet]
        public ActionResult AllUsersList()
        {
            usersMain users = new usersMain();

            return View(users.allUsersList());
        }

        public JsonResult Getfirstname(int id)
        {


            string query = @"select FirstName from emp_profile where user_id = @id";

            string FirstName = con.Query<string>(query, new { id = id }).SingleOrDefault();

            return Json(FirstName);

        }

        public JsonResult Getlastname(int id)
        {


            string query = @"select LastName from emp_profile where user_id = @id";

            string LastName = con.Query<string>(query, new { id = id }).SingleOrDefault();

            return Json(LastName);

        }

        public JsonResult Getusername(int id)
        {


            string query = @"select Email from emp_profile where user_id = @id";

            string UserName = con.Query<string>(query, new { id = id }).SingleOrDefault();

            return Json(UserName);

        }
    }
}