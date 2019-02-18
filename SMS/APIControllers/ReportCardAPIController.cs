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
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        [Route("api/FileAPI/GetReportCard")]
        public HttpResponseMessage GetFile(int sr_number)
        {
            //Create HTTP Response.
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            
            //Read the File into a Byte Array.

            repReport_cardMain rep = new repReport_cardMain();

            byte[] bytes = rep.WebsiteReportCard(sr_number, "2018-19");

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


        [HttpGet]
        [Route("api/FileAPI/GetDuesStatus")]
        public HttpResponseMessage GetDues(int sr_number)
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

                int month_no = con.Query<int>(query).SingleOrDefault();

                if (month_no >= 4 && month_no <= 12)
                {

                    query = @"SELECT 
                                    SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount
                                FROM
                                    out_standing a
                                WHERE
                                    a.sr_number = @sr_number
	                                AND month_no <= @month_no";
                }
                else if (month_no == 1)
                {

                    query = @"SELECT 
                                    SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount
                                FROM
                                    out_standing a
                                WHERE
                                    a.sr_number = @sr_number
	                               and month_no not in (2,3)";
                }
                else if (month_no == 2)
                {


                    query = @"SELECT 
                                    SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount
                                FROM
                                    out_standing a
                                WHERE
                                    a.sr_number = @sr_number
	                                and month_no != 3";

                }
                else
                {

                    query = @"SELECT 
                                    SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount
                                FROM
                                    out_standing a
                                WHERE
                                    a.sr_number = @sr_number";

                }

                decimal dues = con.Query<decimal>(query, new { sr_number = sr_number }).SingleOrDefault();

               response.Content = new StringContent(dues.ToString(), System.Text.Encoding.UTF8, "text/plain");


            return response;
            


          
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
