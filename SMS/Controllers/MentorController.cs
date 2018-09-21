using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class MentorController : BaseController
    {
        [HttpGet]
        public ActionResult AllMentorHeaderList()
        {
           
            mentorMain mentorMain = new mentorMain();
            
            return View(mentorMain.Allmentor_header(Request.Cookies["roles"].Value.ToString(), Request.Cookies["staff_id"].Value.ToString()));

          
        }
        [HttpGet]
        public ActionResult AllMentorDetailList(int mentor_id,int mentor_no, DateTime mentor_date)
        {

            mentorMain mentorMain = new mentorMain();
          
            ViewBag.mentor_no = mentor_no;
            ViewBag.mentor_date = mentor_date.ToString("yyyy-MM-dd");
            ViewBag.mentor_id = mentor_id;
            ViewBag.flag = mentorMain.check_record(mentor_no, mentor_date, mentor_id).ToString();
            return View(mentorMain.Allmentor_detail(ViewBag.mentor_no, ViewBag.mentor_date));
        }

        [HttpGet]
        public ActionResult AssignMentor()
        {
            mentorMain mentorMain = new mentorMain();
            mentor_header mm = new mentor_header();
            mm.dead_line = (System.DateTime.Now.AddMinutes(dateTimeOffSet));
            DDMentor_id();

            return View(mm);


        }

        [HttpPost]
        public ActionResult AssignMentor(mentor_header header)
        {
            mentorMain mentorMain = new mentorMain();
           

            mentorMain.assignMentor(header);


            return RedirectToAction("AllMentorHeaderList");


        }

        [HttpGet]
        public ActionResult addObservation(int mentor_id,int mentor_no, DateTime mentor_date)
        {
            mentorMain mentorMain = new mentorMain();

            mentor_detail detail = new mentor_detail();
            detail.mentor_no = mentor_no;
            detail.mentor_date = mentor_date;
            detail.mentor_id = mentor_id;

            

            return View(detail);


        }

        [HttpGet]
        public ActionResult deleteObservation(int mentor_id,int serial_no, int mentor_no, DateTime mentor_date)
        {
            mentorMain mentorMain = new mentorMain();

            mentor_detail detail = new mentor_detail();

            ViewBag.mentor_id = mentor_id;

            detail =  mentorMain.findObservation(serial_no, mentor_no, mentor_date);


            return View(detail);


        }

        [HttpPost]
        public ActionResult deleteObservation(int mentor_id, int serial_no, int mentor_no, DateTime mentor_date,int j)
        {
            mentorMain mentorMain = new mentorMain();

             mentorMain.deleteObservation(serial_no, mentor_no, mentor_date);


            return RedirectToAction("AllMentorDetailList",new {mentor_id,mentor_no,mentor_date });


        }

        [HttpPost]
        public ActionResult addObservation(mentor_detail detail,int mentor_id)
        {
            mentorMain mentorMain = new mentorMain();

            mentorMain.addObsevation(detail);


           return RedirectToAction("AllMentorDetailList", new { mentor_id = mentor_id,mentor_no = detail.mentor_no, mentor_date = detail.mentor_date });


        }

        public void DDMentor_id()
        {
            mst_staff staff = new mst_staff();
            mst_staffMain main = new mst_staffMain();

            var mentor_list = main.mentor_list();

            IEnumerable<SelectListItem> list2 = new SelectList(mentor_list, "staff_id", "staff_name");

            ViewData["mentor_id"] = list2;
        }

      
    }
}