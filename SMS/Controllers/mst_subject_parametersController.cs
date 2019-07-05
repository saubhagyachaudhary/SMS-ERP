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
    public class mst_subject_parametersController : Controller
    {
        

        [HttpGet]
        public ActionResult AddParameters()
        {
            mst_subjectMain mstSubject = new mst_subjectMain();

            var subject_list = mstSubject.AllSubjectList();

            IEnumerable<SelectListItem> list = new SelectList(subject_list, "subject_id", "subject_name");

            ViewData["subject_id"] = list;


            return View();
        }

        [HttpPost]
        public ActionResult AddParameters(mst_subject_parameters mst)
        {

            try
            {
                mst_subject_parametersMain mstMain = new mst_subject_parametersMain();
                mstMain.AddParameters(mst);
                return RedirectToAction("AllParametersList");
            }
            catch (Exception ex)
            {
                mst_subjectMain mstSubject = new mst_subjectMain();

                var subject_list = mstSubject.AllSubjectList();

                IEnumerable<SelectListItem> list = new SelectList(subject_list, "subject_id", "subject_name");

                ViewData["subject_id"] = list;

                if (mst.parameter_name == null || mst.subject_id == 0)
                {

                    ModelState.AddModelError(String.Empty, "Fields cannot be empty.");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Section Already Created.");
                }
                return View(mst);
            }
        }

        [HttpGet]
        public ActionResult AllParametersList()
        {
            mst_subject_parametersMain stdMain = new mst_subject_parametersMain();

            return View(stdMain.AllParametersList());
        }

        // edit code is perfectly written but commented because of no need : by saubhagya chaudhary

        /*  [HttpGet]
          public ActionResult EditSection(int? id)
          {
              mst_sectionMain stdMain = new mst_sectionMain();

              return View(stdMain.FindSection(id));
          }

          [HttpPost]
          public ActionResult EditSection(mst_section mst)
          {
              mst_sectionMain stdMain = new mst_sectionMain();

              stdMain.EditSection(mst);

              return RedirectToAction("AllSectionList");
          }*/

        [HttpGet]
        public ActionResult DeleteParameter(int id)
        {
            mst_subject_parametersMain stdMain = new mst_subject_parametersMain();

            return View(stdMain.FindParameters(id));
        }

        [HttpPost]
        public ActionResult DeleteParameter(int id, FormCollection collection)
        {
            mst_subject_parametersMain stdMain = new mst_subject_parametersMain();

            //string query = @"SELECT 
            //                    COUNT(*)
            //                FROM
            //                    sr_register a,
            //                    mst_std_section c
            //                WHERE
            //                    c.section_id = @section_id
            //                        AND c.sr_num = a.sr_number
            //                        AND c.session = (SELECT 
            //                            session
            //                        FROM
            //                            mst_session
            //                        WHERE
            //                            session_finalize = 'Y'
            //                                AND session_active = 'Y')";

            //int count = con.Query<int>(query, new { section_id = id }).SingleOrDefault();

            //if (count == 0)
            //{
            //    stdMain.DeleteSection(id);
            //    return RedirectToAction("AllSectionList");
            //}
            //else
            //{
            //    ModelState.AddModelError(String.Empty, "Students assigned cannot delete section");
            //    return View(stdMain.FindSection(id));

            //}

            stdMain.DeleteParameter(id);
            return RedirectToAction("AllParametersList");

        }
    }
}