using Dapper;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_sessionController : Controller
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult AddSession()
        {
            String query = "select count(*) from sms.mst_session where session_active = 'Y'";

            int id = con.ExecuteScalar<int>(query);

            if (id > 0)
            {
                ModelState.AddModelError(String.Empty, "Admission is already open in other session");
                return View();
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddSession(mst_session mst)
        {
            String query = "select count(*) from sms.mst_session where session_active = 'Y'";

            int id = con.ExecuteScalar<int>(query);

            if (id > 0)
            {
                ModelState.AddModelError(String.Empty, "Admission is already open in other session");
                return View();
            }

            if (mst.session_end_date < System.DateTime.Now.AddMinutes(750) && mst.session_active == "Y") 
            {
                ModelState.AddModelError(String.Empty, "You cannot open admission in expired session");
                return View(mst);
            }

            mst_sessionMain mstMain = new mst_sessionMain();

            mstMain.AddSession(mst);

            return RedirectToAction("AllSessionList");
        }

        [HttpGet]
        public ActionResult AllSessionList()
        {
            mst_sessionMain stdMain = new mst_sessionMain();

            return View(stdMain.AllSesssionList());
        }

        [HttpGet]
        public ActionResult EditSession(string id)
        {
            mst_sessionMain stdMain = new mst_sessionMain();

            return View(stdMain.FindSession(id));
        }

        [HttpPost]
        public ActionResult EditSession(mst_session mst)
        {
           
                if (mst.session_end_date < System.DateTime.Now.AddMinutes(750) && mst.session_active == "Y")
                {
                    ModelState.AddModelError(String.Empty, "Session " + mst.session + " already expire you cannot open admission");
                    return View(mst);
                }
                mst_sessionMain stdMain = new mst_sessionMain();

                stdMain.EditSession(mst);

                return RedirectToAction("AllSessionList");
           
        }

        [HttpGet]
        public ActionResult DeleteSession(string id)
        {
            mst_sessionMain stdMain = new mst_sessionMain();

            return View(stdMain.FindSession(id));
        }

        [HttpPost]
        public ActionResult DeleteSession(string id, FormCollection collection)
        {
            try
            {
                mst_sessionMain stdMain = new mst_sessionMain();

                stdMain.DeleteSession(id);

                return RedirectToAction("AllSessionList");
            }
            catch (Exception ex)
            {
                // ModelState.AddModelError(String.Empty, "Sections are created cannot delete");

                return RedirectToAction("AllSessionList");
            }
        }
    }
}