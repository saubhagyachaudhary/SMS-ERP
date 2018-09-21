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
    public class mst_finController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult AddFin()
        {
            String query = "select count(*) from mst_fin where fin_close = 'N'";

            int id = con.ExecuteScalar<int>(query);

            if (id > 0)
            {
                ModelState.AddModelError(String.Empty, "Financial Year is already open");
                return View();
            }

            return View();
        }

        [HttpPost]
        public ActionResult AddFin(mst_fin mst)
        {
            String query = "select count(*) from mst_fin where fin_close = 'N'";

            int id = con.ExecuteScalar<int>(query);

            if (id > 0)
            {
                ModelState.AddModelError(String.Empty, "Financial Year is already open");
                return View();
            }

            if (mst.fin_end_date < System.DateTime.Now.AddMinutes(dateTimeOffSet) && mst.fin_close == "N")
            {
                ModelState.AddModelError(String.Empty, "You cannot open financial year in previous year");
                return View(mst);
            }

            mst_finMain mstMain = new mst_finMain();

            mstMain.AddFin(mst);

            return RedirectToAction("AllFinList");
        }

        [HttpGet]
        public ActionResult AllFinList()
        {
            mst_finMain stdMain = new mst_finMain();

            return View(stdMain.AllFinList());
        }

        [HttpGet]
        public ActionResult EditFin(String id)
        {
            mst_finMain stdMain = new mst_finMain();

            return View(stdMain.FindFin(id));
        }

        [HttpPost]
        public ActionResult EditFin(mst_fin mst)
        {

            if (mst.fin_end_date < System.DateTime.Now.AddMinutes(dateTimeOffSet) && mst.fin_close == "N")
            {
                ModelState.AddModelError(String.Empty, "Financial year already expire");
                return View(mst);
            }
            mst_finMain stdMain = new mst_finMain();

            stdMain.EditFin(mst);

            return RedirectToAction("AllFinList");

        }

        [HttpGet]
        public ActionResult DeleteFin(String id)
        {
            mst_finMain stdMain = new mst_finMain();

            return View(stdMain.FindFin(id));
        }

        [HttpPost]
        public ActionResult DeleteFin(String id, FormCollection collection)
        {
            try
            {
                mst_finMain stdMain = new mst_finMain();

                stdMain.DeleteFin(id);

                return RedirectToAction("AllFinList");
            }
            catch (Exception ex)
            {
                // ModelState.AddModelError(String.Empty, "Sections are created cannot delete");

                return RedirectToAction("AllFinList");
            }
        }
    }
}