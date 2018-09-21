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
    public class mst_transportController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult AddTransport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddTransport(mst_transport mst)
        {
            //string query = "select session_finalize from mst_session where session_active = 'Y'";

            //string id1 = con.ExecuteScalar<string>(query);

            //if (id1 == "Y")
            //{
               

            //    ModelState.AddModelError(String.Empty, "Session is already finalized cannot add new fees.");

            //    return View(mst);
            //}
            //else
            //{
                mst_transportMain mstMain = new mst_transportMain();

                mstMain.AddTransport(mst);

                return RedirectToAction("AllTransportList");
           // }

          
        }

        [HttpGet]
        public ActionResult AllTransportList()
        {
            mst_transportMain stdMain = new mst_transportMain();

            return View(stdMain.AllTransportList());
        }

        [HttpGet]
        public ActionResult EditTransport(int id,string session)
        {
            mst_transportMain stdMain = new mst_transportMain();

            return View(stdMain.FindTransport(id,session));
        }

        [HttpPost]
        public ActionResult EditTransport(mst_transport mst)
        {
            mst_transportMain stdMain = new mst_transportMain();

            stdMain.EditTransport(mst);

            return RedirectToAction("AllTransportList");
        }

        [HttpGet]
        public ActionResult DeleteTransport(int id, string session)
        {
            mst_transportMain stdMain = new mst_transportMain();

            return View(stdMain.FindTransport(id,session));
        }

        [HttpPost]
        public ActionResult DeleteTransport(int id,string session, FormCollection collection)
        {
            try
            {
                mst_transportMain stdMain = new mst_transportMain();

                stdMain.DeleteTransport(id,session);

                return RedirectToAction("AllTransportList");
            }
            catch (Exception ex)
            {
               // ModelState.AddModelError(String.Empty, "Sections are created cannot delete");

                return RedirectToAction("AllTransportList");
            }
        }

    }
}