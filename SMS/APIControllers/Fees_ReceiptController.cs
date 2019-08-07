using Dapper;
using MySql.Data.MySqlClient;
using SMS.report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace SMS.APIControllers
{
    public class Fees_ReceiptController : ApiController
    {
        [HttpGet]
        [Route("api/FileAPI/GetFeesReceipt")]
        public HttpResponseMessage GetFile(string code)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                //Create HTTP Response.
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);

                //Read the File into a Byte Array.

                repFees_receipt rep = new repFees_receipt();

                string query = @"select convert(base64_decode(SUBSTRING_INDEX(substr(reverse(secret_code),length(SUBSTRING_INDEX(reverse(secret_code),'-',1))+2,length(reverse(secret_code))),'-',1)) using utf8) from fees_Receipt where secret_code = @secret_code;";

                int receipt_no = con.Query<int>(query, new { secret_code = code }).First();

                query = @"select date(FROM_UNIXTIME(SUBSTRING_INDEX(reverse(secret_code),'-',1))) from fees_receipt where secret_code = @secret_code;";

                DateTime receipt_date = con.Query<DateTime>(query, new { secret_code = code }).First();

                byte[] bytes = rep.pdf_bytes(receipt_no, receipt_date);

                //Set the Response Content.
                response.Content = new ByteArrayContent(bytes);

                //Set the Response Content Length.
                response.Content.Headers.ContentLength = bytes.LongLength;

                //Set the Content Disposition Header Value and FileName.
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                response.Content.Headers.ContentDisposition.FileName = "Receipt_"+ receipt_no.ToString()+".pdf";

                //Set the File Content Type.
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping("Receipt_" + receipt_no.ToString() + ".pdf"));
                return response;
            }
        }
    }
}
