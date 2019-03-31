using Dapper;
using MySql.Data.MySqlClient;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.AcademicControllers
{
    public class mstSubjectChapterController : Controller
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult AddChapter(string session,int class_id,int subject_id, int section_id)
        {
            mst_chapter chapter = new mst_chapter();

            chapter.class_id = class_id;

            chapter.subject_id = subject_id;

            chapter.section_id = section_id;

            chapter.session = session;

            return View(chapter);
        }

        [HttpPost]
        public ActionResult AddChapter(mst_chapter chapter)
        {
            mst_chapterMain main = new mst_chapterMain();

            main.AddChapter(chapter);

            return RedirectToAction("AllChapterList",new {session = chapter.session, subject_id = chapter.subject_id,class_id = chapter.class_id, section_id = chapter.section_id });
        }

        [HttpGet]
        public ActionResult EditChapter(string session,int class_id,int subject_id,int chapter_id, int section_id)
        {
            mst_chapterMain stdMain = new mst_chapterMain();

            ViewData["Subject_id"] = subject_id;
            ViewData["Class_id"] = class_id;
            ViewData["section_id"] = section_id;
            ViewData["session"] = session;

            return View(stdMain.FindChapter(session, class_id, subject_id, chapter_id,section_id));
        }

        [HttpPost]
        public ActionResult EditChapter(mst_chapter chapter)
        {
            mst_chapterMain stdMain = new mst_chapterMain();

            stdMain.EditChapter(chapter);

            return RedirectToAction("AllChapterList", new { session = chapter.session, subject_id = chapter.subject_id, class_id = chapter.class_id, section_id = chapter.section_id });
        }

        [HttpGet]
        public ActionResult AllSubjectOfTeacher()
        {
            mst_chapterMain stdMain = new mst_chapterMain();

            mst_sessionMain sess = new mst_sessionMain();

            return View(stdMain.AllSubjectOfTeacher(sess.findFinal_Session(), int.Parse(Request.Cookies["loginUserId"].Value.ToString())));
        }

        [HttpGet]
        public ActionResult AllChapterList(string session,int subject_id,int class_id, int section_id)
        {
            mst_chapterMain stdMain = new mst_chapterMain();

           

            ViewData["session"] = session;
            ViewData["subject_id"] = subject_id;
            ViewData["class_id"] = class_id;
            ViewData["section_id"] = section_id;

            return View(stdMain.AllChapterList(session,subject_id,class_id, section_id));
        }

        [HttpGet]
        public ActionResult AllCWWorkList(string session, int subject_id, int class_id, int section_id, int chapter_id)
        {
            mst_class_notebookMain stdMain = new mst_class_notebookMain();
            

            ViewData["session"] = session;
            ViewData["subject_id"] = subject_id;
            ViewData["class_id"] = class_id;
            ViewData["section_id"] = section_id;
            ViewData["chapter_id"] = chapter_id;

            return View(stdMain.AllWorkList(session, subject_id, class_id, section_id, chapter_id, "CW"));
        }

        [HttpGet]
        public ActionResult AllHWWorkList(string session, int subject_id, int class_id, int section_id, int chapter_id)
        {
            mst_class_notebookMain stdMain = new mst_class_notebookMain();
            

            ViewData["session"] = session;
            ViewData["subject_id"] = subject_id;
            ViewData["class_id"] = class_id;
            ViewData["section_id"] = section_id;
            ViewData["chapter_id"] = chapter_id;

            return View(stdMain.AllWorkList(session, subject_id, class_id, section_id, chapter_id, "HW"));
        }

        [HttpGet]
        public ActionResult AddCW(string session,int class_id ,int subject_id,int section_id,int chapter_id)
        {
            mst_class_notebook notebook = new mst_class_notebook();

            notebook.session = session;
            notebook.class_id = class_id;
            notebook.subject_id = subject_id;
            notebook.section_id = section_id;
            notebook.chapter_id = chapter_id;
            notebook.work_type = "CW";

            return View(notebook);
        }

        [HttpPost]
        public ActionResult AddCW(mst_class_notebook notebook)
        {
            mst_class_notebookMain main = new mst_class_notebookMain();

            main.AddWork(notebook);

            return RedirectToAction("AllCWWorkList", new { session = notebook.session, subject_id = notebook.subject_id, class_id = notebook.class_id, section_id = notebook.section_id, chapter_id = notebook.chapter_id });
        }

        [HttpGet]
        public ActionResult AddHW(string session, int class_id, int subject_id, int section_id, int chapter_id)
        {
            mst_class_notebook notebook = new mst_class_notebook();

            notebook.session = session;
            notebook.class_id = class_id;
            notebook.subject_id = subject_id;
            notebook.section_id = section_id;
            notebook.chapter_id = chapter_id;
            notebook.work_type = "HW";

            return View(notebook);
        }

        [HttpPost]
        public ActionResult AddHW(mst_class_notebook notebook)
        {
            mst_class_notebookMain main = new mst_class_notebookMain();

            main.AddWork(notebook);

            return RedirectToAction("AllHWWorkList", new { session = notebook.session, subject_id = notebook.subject_id, class_id = notebook.class_id, section_id = notebook.section_id, chapter_id = notebook.chapter_id });
        }

        [HttpGet]
        public ActionResult DeleteCW(string session, int class_id, int subject_id, int section_id, int chapter_id, int work_id)
        {
            mst_class_notebook notebook = new mst_class_notebook();
            mst_class_notebookMain main = new mst_class_notebookMain();

            notebook.session = session;
            notebook.class_id = class_id;
            notebook.subject_id = subject_id;
            notebook.section_id = section_id;
            notebook.chapter_id = chapter_id;
            notebook.work_type = "CW";
            notebook.work_id = work_id;

            return View(main.findWork(notebook));
        }

        [HttpPost]
        public ActionResult DeleteCW(mst_class_notebook notebook)
        {
            mst_class_notebookMain main = new mst_class_notebookMain();

            notebook.work_type = "CW";
            main.deleteWork(notebook);

            return RedirectToAction("AllCWWorkList", new { session = notebook.session, subject_id = notebook.subject_id, class_id = notebook.class_id, section_id = notebook.section_id, chapter_id = notebook.chapter_id });
        }

        [HttpGet]
        public ActionResult DeleteHW(string session, int class_id, int subject_id, int section_id, int chapter_id, int work_id)
        {
            mst_class_notebook notebook = new mst_class_notebook();
            mst_class_notebookMain main = new mst_class_notebookMain();

            notebook.session = session;
            notebook.class_id = class_id;
            notebook.subject_id = subject_id;
            notebook.section_id = section_id;
            notebook.chapter_id = chapter_id;
            notebook.work_type = "HW";
            notebook.work_id = work_id;

            return View(main.findWork(notebook));
        }

        [HttpPost]
        public ActionResult DeleteHW(mst_class_notebook notebook)
        {
            mst_class_notebookMain main = new mst_class_notebookMain();

            notebook.work_type = "HW";
            main.deleteWork(notebook);

            return RedirectToAction("AllHWWorkList", new { session = notebook.session, subject_id = notebook.subject_id, class_id = notebook.class_id, section_id = notebook.section_id, chapter_id = notebook.chapter_id });
        }

        [HttpGet]
        public ActionResult Student_Copy(string session, int subject_id, int class_id, int section_id)
        {
            
            copy_correction copy = new copy_correction();

            copy.subject_id = subject_id;
            copy.session = session;
            copy.class_id = class_id;
            copy.section_id = section_id;
            
            return View(copy);
        }

        [HttpPost]
        public ActionResult Student_Copy(copy_correction copy)
        {
            string query = @"SELECT 
                                    COUNT(*)
                                FROM
                                    mst_std_class a,
                                    mst_std_section b
                                WHERE
                                    a.session = b.session
                                        AND a.sr_num = b.sr_num
                                        AND a.class_id = @class_id
                                        AND b.section_id = @section_id
                                        AND a.sr_num = @sr_number";

            int flag = con.Query<int>(query, new { sr_number = copy.sr_number,class_id = copy.class_id, section_id = copy.section_id }).SingleOrDefault();

            if (flag == 0)
            {
                ModelState.AddModelError(String.Empty, "Student not found.");
                return View(copy);
            } 

            return RedirectToAction("AllUncheckedChapters", new { sr_number = copy.sr_number,session = copy.session, subject_id = copy.subject_id, class_id = copy.class_id, section_id = copy.section_id });
        }

        [HttpGet]
        public ActionResult AllUncheckedChapters(int sr_number,string session, int subject_id, int class_id, int section_id)
        {
            copy_correctionMain main = new copy_correctionMain();
            copy_correction copy = new copy_correction();

            copy.sr_number = sr_number;
            copy.session = session;
            copy.subject_id = subject_id;
            copy.class_id = class_id;
            copy.section_id = section_id;

            
            ViewData["session"] = session;
            ViewData["subject_id"] = subject_id;
            ViewData["class_id"] = class_id;
            ViewData["section_id"] = section_id;
            ViewData["sr_number"] = sr_number;

            return View(main.AllUncheckedChapterList(copy));
        }

        [HttpGet]
        public ActionResult AllUncheckedQuestions(int sr_number,string session ,int class_id ,int subject_id ,int chapter_id ,int section_id )
        {
            copy_correctionMain main = new copy_correctionMain();
            copy_correction copy = new copy_correction();

            copy.sr_number = sr_number;
            copy.session = session;
            copy.subject_id = subject_id;
            copy.class_id = class_id;
            copy.section_id = section_id;
            copy.chapter_id = chapter_id;


            ViewData["session"] = session;
            ViewData["subject_id"] = subject_id;
            ViewData["class_id"] = class_id;
            ViewData["section_id"] = section_id;
            ViewData["sr_number"] = sr_number;

            var result = main.AllUncheckedQuestions(copy);

            foreach (var corr in result)
            {
                corr.sr_number = sr_number;
                corr.chapter_id = chapter_id;
                corr.teacher_correctby= int.Parse(Request.Cookies["loginUserId"].Value.ToString());
                corr.class_id = class_id;
                corr.section_id = section_id;
                corr.subject_id = subject_id;
            }

            return View(result);
        }

        [HttpPost]
        public ActionResult AddCheckedQuestion(IEnumerable<copy_correction> correction)
        {
            copy_correctionMain main = new copy_correctionMain();

            main.AddCheckedQuestion(correction);

            int sr_number = 0 ;
            string session="";
            int class_id=0;
            int subject_id=0;
            int chapter_id=0;
            int section_id=0;
            foreach (var i in correction)
            {
                sr_number = i.sr_number;
                session = i.session;
                class_id = i.class_id;
                subject_id = i.subject_id;
                chapter_id = i.chapter_id;
                section_id = i.section_id;
                break;
            }

            return RedirectToAction("Student_Copy", new {  session= session, class_id= class_id, subject_id= subject_id,section_id = section_id });
        }

        
    }

    public class copy_correction
    {
        [Key]
        [Display(Name = "Session")]
        [Required]
        public string session { get; set; }

        [Key]
        [Display(Name = "Admission No.")]
        [Required]
        public int sr_number { get; set; }

        [Key]
        [Display(Name = "Work")]
        [Required]
        public string work_id { get; set; }

        [Display(Name = "Work")]
        public string work_name { get; set; }

        [Display(Name = "Class Name")]
        [Required]
        public int class_id { get; set; }

        [Display(Name = "Section Name")]
        [Required]
        public int section_id { get; set; }

        [Display(Name = "Chapter Name")]
        [Required]
        public int chapter_id { get; set; }

        [Display(Name = "Subject Name")]
        [Required]
        public int subject_id { get; set; }

        [Display(Name = "Correction Date")]
        [Required]
        public DateTime correction_date { get; set; }

        
        [Display(Name = "Correct")]
        [Required]
        public bool que_correct { get; set; }

        [Display(Name = "Remark")]
        public string remark { get; set; }

        [Display(Name = "Work Type")]
        public string work_type { get; set; }

        [Display(Name = "Work")]
        [Required]
        public int teacher_correctby { get; set; }

        public bool work_done { get; set; }

        [Display(Name = "Work Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime work_date { get; set; }


    }

    public class copy_correctionMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public IEnumerable<mst_chapter> AllUncheckedChapterList(copy_correction copy)
        {
            string query = @"SELECT 
                                    session,
                                    class_id,
                                    section_id,
                                    subject_id,
                                    chapter_id,
                                    chapter_name,
                                    chapter_start_date,
                                    chapter_end_date
                                FROM
                                    mst_chapter
                                WHERE
                                    CONCAT(class_id,
                                            section_id,
                                            subject_id,
                                            chapter_id) IN (SELECT DISTINCT
                                            CONCAT(class_id,
                                                        section_id,
                                                        subject_id,
                                                        chapter_id)
                                        FROM
                                            mst_class_notebook
                                        WHERE
                                            work_id NOT IN (SELECT 
                                                    work_id
                                                FROM
                                                    copy_correction
                                                WHERE
                                                    sr_number = @sr_number AND session = @session)
                                                AND session = @session)
                                        AND session = @session
                                        AND subject_id = @subject_id
                                        AND class_id = @class_id
                                        AND section_id = @section_id";

            var result = con.Query<mst_chapter>(query, new { sr_number = copy.sr_number, session = copy.session, subject_id = copy.subject_id ,class_id = copy.class_id,section_id = copy.section_id });

            return result;
        }

        public IEnumerable<copy_correction>  AllUncheckedQuestions(copy_correction copy)
        {
            string query = @"SELECT 
                                session,
                                class_id,
                                section_id,
                                subject_id,
                                chapter_id,
                                work_type,
                                work_id,
                                work_name,
                                work_date
                            FROM
                                mst_class_notebook
                            WHERE
                                work_id NOT IN (SELECT 
                                        work_id
                                    FROM
                                        copy_correction
                                    WHERE
                                        sr_number = @sr_number AND session = @session)
                                    AND session = @session
                                    AND subject_id = @subject_id
                                    AND class_id = @class_id
                                    AND section_id = @section_id
                                    AND chapter_id = @chapter_id
                            ORDER BY work_date DESC , work_type";

            var result = con.Query<copy_correction>(query, new { sr_number = copy.sr_number, session = copy.session, subject_id = copy.subject_id, class_id = copy.class_id, section_id = copy.section_id,chapter_id = copy.chapter_id });
            
            return result;
        }


        public void AddCheckedQuestion(IEnumerable<copy_correction> copy)
        {

            try
            {

                string query = @"INSERT INTO `copy_correction`
                                (`session`,
                                `sr_number`,
                                `work_id`,
                                `correction_date`,
                                `que_correct`,
                                `remark`,
                                `teacher_correctby`)
                                VALUES
                                (@session,
                                @sr_number,
                                @work_id,
                                curdate(),
                                @que_correct,
                                @remark,
                                @teacher_correctby)";
                foreach (var ob in copy)
                {
                    if (ob.work_done)
                    {
                        
                        con.Execute(query, ob);
                    }
                }

            }
            catch(Exception ex)
            {
                //do exception handling
            }
        }

    }

    public class mst_chapter
    {
        [Key]
        [Display(Name ="Session")]
        [Required]
        public string session{ get; set; }

        [Key]
        [Display(Name = "Class Name")]
        [Required]
        public int class_id { get; set; }

        [Key]
        [Display(Name = "Section Name")]
        [Required]
        public int section_id { get; set; }

        [Key]
        [Display(Name = "Subject Name")]
        [Required]
        public int subject_id { get; set; }

        [Key]
        [Display(Name = "Chapter Name")]
        [Required]
        public int chapter_id { get; set; }

        [Display(Name = "Subject Name")]
        [Required]
        public string subject_name { get; set; }

        [Display(Name = "Chapter Name")]
        [Required]
        public string chapter_name { get; set; }

        [Display(Name = "Class Name")]
        [Required]
        public string class_name { get; set; }

        [Display(Name = "Section Name")]
        [Required]
        public string section_name { get; set; }

        [Display(Name ="Start Date")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime chapter_start_date { get; set; }

        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? chapter_end_date { get; set; }


        [Display(Name = "Chapter Close")]
        public bool chapter_close { get; set; }
    }
    
    public class mst_chapterMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddChapter(mst_chapter chapter)
        {
            try
            {
                string query = @"INSERT INTO mst_chapter
                                    (session,
                                    class_id,
                                    section_id,
                                    subject_id,
                                    chapter_id,
                                    chapter_name,
                                    chapter_start_date)
                                    VALUES
                                    (@session,
                                    @class_id,
                                    @section_id,
                                    @subject_id,
                                    @chapter_id,
                                    @chapter_name,
                                    curdate())";

                string maxid = @"SELECT 
                                        IFNULL(MAX(chapter_id), 0) + 1
                                    FROM
                                        mst_chapter
                                    WHERE
                                        session = @session
                                            AND subject_id = @subject_id
                                            AND class_id = @class_id
                                            AND section_id = @section_id";

                chapter.chapter_id = con.ExecuteScalar<int>(maxid,new {session = chapter.session,subject_id = chapter.subject_id, class_id = chapter.class_id, section_id = chapter.section_id });

                con.Execute(query,chapter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_chapter> AllChapterList(string session,int subject_id,int class_id,int section_id)
        {
            string query = @"SELECT 
                                session,
                                class_id,
                                section_id,
                                subject_id,
                                chapter_id,
                                chapter_name,
                                chapter_start_date,
                                chapter_end_date
                            FROM
                                mst_chapter
                            WHERE
                                session = @session
                                    AND class_id = @class_id
                                    AND section_id = @section_id
                                    AND subject_id = @subject_id";

            var result = con.Query<mst_chapter>(query, new { session = session, class_id= class_id , subject_id = subject_id, section_id = section_id });

            return result;
        }

        public IEnumerable<mst_chapter> AllSubjectOfTeacher(string session, int subject_teacher_id)
        {
            string query = @"SELECT 
                                a.session,
                                a.class_id,
                                a.subject_id,
                                subject_name,
                                class_name,
                                a.section_id,
                                d.section_name
                            FROM
                                mst_class_subject_teacher a,
                                mst_subject b,
                                mst_class c,
                                mst_section d
                            WHERE
                                a.session = b.session
                                    AND b.session = c.session
                                    AND c.session = d.session
                                    AND d.session = @session
                                    AND a.subject_id = b.subject_id
                                    AND a.class_id = c.class_id
                                    AND a.section_id = d.section_id
                                    AND a.subject_teacher_id = @subject_teacher_id
                                    order by c.order_by";

            var result = con.Query<mst_chapter>(query, new { session = session, subject_teacher_id= subject_teacher_id });

            return result;
        }

        public mst_chapter FindChapter(string session ,int class_id, int subject_id, int chapter_id, int section_id)
        {
            string Query = @"SELECT 
                                    Chapter_name
                                FROM
                                    mst_chapter
                                WHERE
                                    session = @session
                                        AND class_id = @class_id
                                        AND subject_id = @subject_id
                                        AND chapter_id = @chapter_id
                                        AND section_id = @section_id";

            return con.Query<mst_chapter>(Query, new {session = session, class_id = class_id, subject_id= subject_id, chapter_id = chapter_id, section_id = section_id }).SingleOrDefault();
        }

        public void EditChapter(mst_chapter mst)
        {

            try
            {
                if(mst.chapter_close)
                {
                    mst.chapter_end_date = System.DateTime.Now;
                }
                else
                {
                    mst.chapter_end_date = null;
                }

                string query = @"UPDATE mst_chapter 
                                    SET 
                                        chapter_name = @chapter_name,
                                        chapter_end_date = @chapter_end_date
                                    WHERE
                                        session = @session
                                            AND subject_id = @subject_id
                                            AND chapter_id = @chapter_id
                                            AND section_id = @section_id";

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public class mst_class_notebook
    {
        [Key]
        [Display(Name = "Session")]
        [Required]
        public string session { get; set; }

        [Key]
        [Display(Name = "Class Name")]
        [Required]
        public int class_id { get; set; }

        [Key]
        [Display(Name = "Section Name")]
        [Required]
        public int section_id { get; set; }

        [Key]
        [Display(Name = "Subject Name")]
        [Required]
        public int subject_id { get; set; }

        [Key]
        [Display(Name = "Chapter Name")]
        [Required]
        public int chapter_id { get; set; }

        [Key]
        [Display(Name = "Work Done")]
        [Required]
        public int work_id { get; set; }

        [Display(Name = "Work Done")]
        [Required]
        public string work_name { get; set; }

        [Display(Name = "Subject Name")]
        [Required]
        public string subject_name { get; set; }

        [Display(Name = "Chapter Name")]
        [Required]
        public string chapter_name { get; set; }

        [Display(Name = "Class Name")]
        [Required]
        public string class_name { get; set; }

        [Display(Name = "Section Name")]
        [Required]
        public string section_name { get; set; }

        [Display(Name = "Work Date")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime work_date { get; set; }

        [Key]
        [Display(Name = "Work Type")]
        [Required]
        public string work_type { get; set; }

       
    }

    public class mst_class_notebookMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddWork(mst_class_notebook notbook)
        {
            try
            {
               
                string query = @"INSERT INTO `mst_class_notebook`
                                (`session`,
                                `class_id`,
                                `section_id`,
                                `subject_id`,
                                `chapter_id`,
                                `work_id`,
                                `work_name`,
                                `work_date`,
                                `work_type`)
                                VALUES
                                (@session,
                                @class_id,
                                @section_id,
                                @subject_id,
                                @chapter_id,
                                @work_id,
                                @work_name,
                                curdate(),
                                @work_type)";

                string maxid = @"SELECT 
                                        IFNULL(MAX(work_id), 0) + 1
                                    FROM
                                        mst_class_notebook
                                    WHERE
                                        session = @session";

                notbook.work_id = con.ExecuteScalar<int>(maxid, new { session = notbook.session});

                con.Execute(query, notbook);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_class_notebook> AllWorkList(string session, int subject_id, int class_id, int section_id, int chapter_id, string work_type)
        {
            string query = @"SELECT 
                                session,
                                class_id,
                                section_id,
                                chapter_id,
                                work_id,
                                work_name,
                                work_date,
                                work_type,
                                subject_id
                            FROM
                                mst_class_notebook
                            WHERE
                                session = @session
                                    AND class_id = @class_id
                                    AND section_id = @section_id
                                    AND subject_id = @subject_id
                                    AND chapter_id = @chapter_id
                                    AND work_type = @work_type";

            var result = con.Query<mst_class_notebook>(query, new { session = session, class_id = class_id, subject_id = subject_id, section_id = section_id,chapter_id = chapter_id, work_type = work_type });

            return result;
        }

        public void deleteWork(mst_class_notebook work)
        {
            try
            {

                string query = @"DELETE FROM `mst_class_notebook` 
                                WHERE
                                    session = @session
                                    AND class_id = @class_id
                                    AND section_id = @section_id
                                    AND subject_id = @subject_id
                                    AND chapter_id = @chapter_id
                                    AND work_id = @work_id
                                    AND work_type = @work_type";

                var result = con.Query<mst_class_notebook>(query, work);
            }
            catch (Exception ex)
            {
                // do whatever is required
            }
        }

        public mst_class_notebook findWork(mst_class_notebook notebook)
        {
            string query = @"SELECT 
                                session,
                                class_id,
                                section_id,
                                chapter_id,
                                work_id,
                                work_name,
                                work_date,
                                work_type,
                                subject_id
                            FROM
                                mst_class_notebook
                            WHERE
                                session = @session
                                    AND class_id = @class_id
                                    AND section_id = @section_id
                                    AND subject_id = @subject_id
                                    AND chapter_id = @chapter_id
                                    AND work_type = @work_type
                                    AND work_id = @work_id";

            var result = con.Query<mst_class_notebook>(query,notebook).SingleOrDefault();

            return result;
        }
    }
}