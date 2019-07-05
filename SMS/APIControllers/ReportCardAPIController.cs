using Dapper;
using MySql.Data.MySqlClient;
using SMS.report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace SMS.APIControllers
{
    public class ReportCardAPIController : ApiController
    {


        [HttpGet]
        [Route("api/FileAPI/GetReportCard")]
        public HttpResponseMessage GetFile(int sr_number)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                //Create HTTP Response.
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

                //Read the File into a Byte Array.

                repReport_cardMain rep = new repReport_cardMain();

                string query = @"SELECT
                                    b.session
                                FROM
                                    mst_std_class a,
                                    website_declare b
                                WHERE
                                    a.session = b.session
                                        AND a.class_id = b.class_id
                                        AND a.sr_num = @sr_number
                                        AND CURDATE() BETWEEN b.declare_from AND b.declare_to";

                string session = con.Query<string>(query, new { sr_number = sr_number }).SingleOrDefault();

                byte[] bytes = rep.WebsiteReportCard(sr_number, session);

                //Set the Response Content.
                response.Content = new ByteArrayContent(bytes);

                //Set the Response Content Length.
                response.Content.Headers.ContentLength = bytes.LongLength;

                //Set the Content Disposition Header Value and FileName.
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Report_Card.pdf";

                //Set the File Content Type.
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Report_Card.pdf"));
                return response;
            }
        }


        [HttpGet]
        [Route("api/FileAPI/GetDuesStatus")]
        public HttpResponseMessage GetDues(int sr_number)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                //Create HTTP Response.
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

                //Set the File Path.
                //string filePath = HttpContext.Current.Server.MapPath("~/images/") + fileName;

                //Check whether File exists.

                string query = @"SELECT 
                                    website_code,
                                    b.session,
                                    declare_from,
                                    declare_to,
                                    b.class_id,
                                    dues_month_no
                                FROM
                                    mst_std_class a,
                                    website_declare b
                                WHERE
                                    a.session = b.session
                                        AND a.class_id = b.class_id
                                        AND a.sr_num = @sr_number
                                        AND CURDATE() BETWEEN b.declare_from AND b.declare_to";

                website_declare declare = con.Query<website_declare>(query, new { sr_number = sr_number }).SingleOrDefault();

                if (declare == null)
                {
                    response.Content = new StringContent(String.Format("Report card of admission number {0} is not yet declared. Thank You", sr_number.ToString()), System.Text.Encoding.UTF8, "text/plain");

                    return response;
                }

                int month_no = declare.dues_month_no;

                if (month_no >= 4 && month_no <= 12)
                {

                    query = @"SELECT 
                                    SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount
                                FROM
                                    out_standing a
                                WHERE
                                    a.sr_number = @sr_number
	                                AND month_no <= @month_no
                                    AND session = @session
                                    AND acc_id != 6";
                }
                else if (month_no == 1)
                {

                    query = @"SELECT 
                                    SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount
                                FROM
                                    out_standing a
                                WHERE
                                    a.sr_number = @sr_number
	                               and month_no not in (2,3)
                                    AND session = @session
                                    AND acc_id != 6";
                }
                else if (month_no == 2)
                {


                    query = @"SELECT 
                                    SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount
                                FROM
                                    out_standing a
                                WHERE
                                    a.sr_number = @sr_number
	                                and month_no != 3
                                    AND session = @session
                                    AND acc_id != 6";

                }
                else
                {

                    query = @"SELECT 
                                    SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount
                                FROM
                                    out_standing a
                                WHERE
                                    a.sr_number = @sr_number
                                    AND session = @session
                                    AND acc_id != 6";

                }

                decimal dues = con.Query<decimal>(query, new { sr_number = sr_number, session = declare.session }).SingleOrDefault();

                if (dues == 0m)
                    response.Content = new StringContent("True", System.Text.Encoding.UTF8, "text/plain");
                else
                    response.Content = new StringContent(String.Format("Note: Account of admission number {0} show's some dues kindly clear it in order to download the report card.", sr_number.ToString()), System.Text.Encoding.UTF8, "text/plain");

                return response;



            }
        }

        [HttpGet]
        [Route("api/FileAPI/checkEnable")]
        public HttpResponseMessage checkEnable()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                //Create HTTP Response.
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

                //Set the File Path.
                //string filePath = HttpContext.Current.Server.MapPath("~/images/") + fileName;

                //Check whether File exists.

                string query = @"SELECT 
                                COUNT(*)
                            FROM
                                website_declare
                            WHERE
                                CURDATE() BETWEEN declare_from AND declare_to";

                int check = con.Query<int>(query).SingleOrDefault();


                if (check == 0)
                    response.Content = new StringContent("False", System.Text.Encoding.UTF8, "text/plain");
                else
                    response.Content = new StringContent("True", System.Text.Encoding.UTF8, "text/plain");

                return response;

            }


        }
    }

    public class website_declare
    {
        public string website_code { get; set; }

        public string session { get; set; }

        public DateTime declare_from { get; set; }

        public DateTime declare_to { get; set; }

        public int class_id { get; set; }

        public int dues_month_no { get; set; }

    }
}
