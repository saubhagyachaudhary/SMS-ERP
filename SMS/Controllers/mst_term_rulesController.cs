using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using Dapper;

namespace SMS.Controllers
{
    public class mst_term_rulesController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult AddTermRules()
        {
            mst_classMain mstClass = new mst_classMain();
            mst_termMain mstterm = new mst_termMain();

            var term_list = mstterm.AllTermList();

            mst_sessionMain sess = new mst_sessionMain();

            var class_list = mstClass.AllClassList(sess.findFinal_Session());

           
            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
            IEnumerable<SelectListItem> list1 = new SelectList(term_list, "term_id", "term_name");


            ViewData["class_id"] = list;
            ViewData["term_id"] = list1;

            return View();
        }

        [HttpPost]
        public ActionResult AddTermRules(mst_term_rules mst)
        {
            mst_term_rulesMain mstMain = new mst_term_rulesMain();

            mstMain.AddTermRule(mst);

            return RedirectToAction("ALLTermRuleList");
        }

        public JsonResult GetExam(int id)
        {
            string query = @"SELECT 
                                b.exam_id, b.exam_name
                            FROM
                                mst_exam_class a,
                                mst_exam b
                            WHERE
                                a.exam_id = b.exam_id AND a.class_id = @class_id
                                    AND a.session = b.session
                                    AND a.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y'
                                            AND session_active = 'Y')";


            var exam_list = con.Query<mst_exam>(query, new { class_id = id });

            IEnumerable<SelectListItem> list = new SelectList(exam_list, "exam_id", "exam_name");

            return Json(list);

        }

        [HttpGet]
        public ActionResult ALLTermRuleList()
        {
            mst_term_rulesMain mstMain = new mst_term_rulesMain();

            return View(mstMain.AllTermRuleList());
        }

        [HttpGet]
        public ActionResult DeleteTermRule(string session,int term_id,int class_id,int evaluation_id)
        {
            mst_term_rulesMain mstMain = new mst_term_rulesMain();
            
            return View(mstMain.FindTermRule(class_id,term_id,session, evaluation_id));
        }

        [HttpPost]
        public ActionResult DeleteTermRule(mst_term_rules term , FormCollection form)
        {
            mst_term_rulesMain mstMain = new mst_term_rulesMain();

            mstMain.DeleteTermRule(term);

            return RedirectToAction("ALLTermRuleList");
        }
    }
}