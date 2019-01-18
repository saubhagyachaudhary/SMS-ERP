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
    public class mst_feesController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult AddFees()
        {
            mst_classMain mstClass = new mst_classMain();
            mst_feesMain mstfess = new mst_feesMain();

            mst_sessionMain sess = new mst_sessionMain();

            var class_list = mstClass.AllClassList(sess.findActive_Session());

            var acc_head = mstfess.account_head();

            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
            IEnumerable<SelectListItem> list1 = new SelectList(acc_head, "acc_id", "acc_name");


            ViewData["class_id"] = list;
            ViewData["acc_id"] = list1;

            return View();
        }

        [HttpPost]
        public ActionResult AddFees(mst_fees mst)
        {

            try
            {
                string query = "select session_finalize from mst_session where session_active = 'Y'";

                string id1 = con.ExecuteScalar<string>(query);
                if(id1 == "Y")
                {
                    mst_classMain mstClass = new mst_classMain();
                    mst_feesMain mstfess = new mst_feesMain();

                    mst_sessionMain sess = new mst_sessionMain();

                    var class_list = mstClass.AllClassList(sess.findActive_Session());

                    var acc_head = mstfess.account_head();

                    IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
                    IEnumerable<SelectListItem> list1 = new SelectList(acc_head, "acc_id", "acc_name");


                    ViewData["class_id"] = list;
                    ViewData["acc_id"] = list1;

                    ModelState.AddModelError(String.Empty, "Session is already finalized cannot add new fees.");

                    return View(mst);
                }
                else
                {
                    mst_feesMain mstMain = new mst_feesMain();
                    mstMain.AddFees(mst);
                    return RedirectToAction("AllFeesList");
                }
               
            }
            catch (Exception ex)
            {
                mst_classMain mstClass = new mst_classMain();

                mst_sessionMain sess = new mst_sessionMain();

                var class_list = mstClass.AllClassList(sess.findActive_Session());
                IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");

                ViewData["class_id"] = list;

                ModelState.AddModelError(String.Empty, "Section Already Created");

                return View(mst);
            }
        }



        [HttpGet]
        public ActionResult AllFeesList()
        {
            mst_feesMain stdMain = new mst_feesMain();

            return View(stdMain.AllFeesList());
        }

        [HttpGet]
        public ActionResult EditFees(int class_id, int acc_id,string session)
        {
            mst_feesMain stdMain = new mst_feesMain();

            return View(stdMain.Findfees(class_id, acc_id,session));
        }

        [HttpPost]
        public ActionResult EditFees(mst_fees mst)
        {
            mst_feesMain stdMain = new mst_feesMain();

            string query = @"select session_finalize from mst_session where session_active = 'Y'";

            string id = con.Query<string>(query).SingleOrDefault();

            if(id == "N")
            {
                stdMain.EditFees(mst);
                return RedirectToAction("AllFeesList");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Fees are already finalize cannot edit.");
                return View(mst);
            }

            

            
        }

        [HttpGet]
        public ActionResult DeleteFees(int class_id, int acc_id,string session)
        {
            mst_feesMain stdMain = new mst_feesMain();

            return View(stdMain.Findfees(class_id, acc_id,session));
        }

        [HttpPost]
        public ActionResult DeleteFees(int class_id, int acc_id,string session ,FormCollection collection)
        {
            mst_feesMain stdMain = new mst_feesMain();

            string query = @"select session_finalize from mst_session where session_active = 'Y'";

            string id = con.Query<string>(query).SingleOrDefault();

            if (id == "N")
            {
                stdMain.DeleteFees(class_id, acc_id,session);
                return RedirectToAction("AllFeesList");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Fees is already finalize cannot delete.");
                return View(stdMain.Findfees(class_id, acc_id, session));
            }

            

            
        }
    }
}