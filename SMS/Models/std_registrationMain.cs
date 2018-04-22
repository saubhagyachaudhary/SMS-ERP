using Dapper;
using SMS.Models;
using SMS.report;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class std_registrationMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddRegistration(std_registration std)
        {
            try
            {
                mst_fin fin = new mst_fin();

                string query1 = @"SELECT fin_id
                              ,fin_start_date
                              ,fin_end_date
                              ,fin_close
                          FROM sms.mst_fin
                          where fin_close = 'N'";

                fin = con.Query<mst_fin>(query1).SingleOrDefault();

                if (std.reg_date > fin.fin_start_date && std.reg_date < fin.fin_end_date)
                {
                    string sess = "select session from sms.mst_session where session_active = 'Y'";

                    string session = con.ExecuteScalar<string>(sess);

                    string maxid = "select ifnull(MAX(reg_no),0)+1 from sms.sr_register where adm_session = @adm_session";

                    int id = con.Query<int>(maxid, new { adm_session = session }).SingleOrDefault();

                    string max = "select ifnull(MAX(reg_no),0)+1 from sms.std_registration where session = @adm_session";

                    int id1 = con.Query<int>(max, new { adm_session = session }).SingleOrDefault();




                    string query = @"INSERT INTO sms.std_registration
           (session
           ,reg_no
           ,reg_date
           ,std_first_name
           ,std_last_name
           ,std_father_name
           ,std_mother_name
           ,std_address
           ,std_address1
           ,std_address2
           ,std_district
           ,std_state
           ,std_country
           ,std_pincode
           ,std_contact
           ,std_contact1
           ,std_contact2
           ,std_email
           ,std_class_id)
     VALUES
           (@session
           ,@reg_no
           ,@reg_date
           ,@std_first_name
           ,@std_last_name
           ,@std_father_name
           ,@std_mother_name
           ,@std_address
           ,@std_address1
           ,@std_address2
           ,@std_district
           ,@std_state
           ,@std_country
           ,@std_pincode
           ,@std_contact
           ,@std_contact1
           ,@std_contact2
           ,@std_email
           ,@std_class_id)";

                    std.session = session;

                    if (id1 < id)
                    {
                        std.reg_no = id;
                    }
                    else
                    {
                        std.reg_no = id1;
                    }
                    std.reg_date = System.DateTime.Now.AddMinutes(750);



                    con.Execute(query,
                            new
                            {
                                std.session
                               ,
                                std.reg_no
                               ,
                                std.reg_date
                               ,
                                std.std_first_name
                               ,
                                std.std_last_name
                               ,
                                std.std_father_name
                               ,
                                std.std_mother_name
                               ,
                                std.std_address
                               ,
                                std.std_address1
                               ,
                                std.std_address2
                               ,
                                std.std_district
                               ,
                                std.std_state
                               ,
                                std.std_country
                               ,
                                std.std_pincode
                               ,
                                std.std_contact
                               ,
                                std.std_contact1
                               ,
                                std.std_contact2
                               ,
                                std.std_email
                               ,
                                std.std_class_id


                            });

                    out_standing out_std= new out_standing();

                    out_std.acc_id = 1;
                    out_std.outstd_amount = std.fees_amount;
                    out_std.reg_num = std.reg_no;

                    out_standingMain out_stdMain = new out_standingMain();

                    out_stdMain.AddOutStanding(out_std);

                    SMSMessage sms = new SMSMessage();

                    string qry = "select class_name from sms.mst_class where class_id = @class_id";

                    string className = con.Query<string>(qry, new { class_id = std.std_class_id }).SingleOrDefault();

                    string text =  std.std_first_name+" "+ std.std_last_name+" is successfully registered in class "+ className + @". This registration is valid for 3 days subject to availability of seats. Thank You. Hariti Public School.";

                    sms.SendSMS(text, std.std_contact);

                     text = std.std_first_name + " " + std.std_last_name + " का पंजीकरण कक्षा " + className + " में सफलतापूर्वक हो गया है। यह पंजीकरण 3 दिन तक मान्य रहेगा। कक्षा में सीटों की उपलब्धता सीमित हैं। धन्यवाद। Hariti Public School";

                    sms.SendSMS(text, std.std_contact);

                }
                
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<std_registration> AllRegistrationList()
        {
            String query = @"select session,reg_no,reg_date,std_first_name,std_last_name,std_father_name,c.class_name
                                from sms.std_registration a,sms.mst_class c
                                where
                                a.std_class_id = c.class_id order by reg_date desc";

            var result = con.Query<std_registration>(query);

            return result;
        }

        public std_registration FindRegistration(string sess, int reg, DateTime reg_dt)
        {
            String Query = @"SELECT session
							  ,reg_no
							  ,reg_date
                              ,std_first_name
                              ,std_last_name
                              ,std_father_name
                              ,std_mother_name
                              ,std_address
                              ,std_address1
                              ,std_address2
                              ,std_district
                              ,std_state
                              ,std_country
                              ,std_pincode
                              ,std_contact
                              ,std_contact1
                              ,std_contact2
                              ,std_email
                              ,c.class_name
                              ,c.class_id std_class_id
                             FROM sms.std_registration a,sms.mst_class c
							  where
							 a.std_class_id = c.class_id 
							 and
							 a.session = @session
							 and
							 a.reg_no = @reg_no
							 and
							 a.reg_date = @reg_date";

            return con.Query<std_registration>(Query, new { session = sess, reg_no = reg , reg_date = reg_dt }).SingleOrDefault();
        }

        public std_registration FindRegistrationForFees(int reg)
        {
            String Query = @"SELECT 
                              std_first_name
                              ,std_last_name
                              ,std_father_name
                              ,std_mother_name
                              ,std_contact
                              ,std_email
                              ,c.class_name
                             FROM sms.std_registration a,sms.mst_class c
							  where
							 a.std_class_id = c.class_id 
							 and
								a.reg_no = @reg_no";

            return con.Query<std_registration>(Query, new {reg_no = reg}).SingleOrDefault();
        }

        public void EditRegistration(std_registration std)
        {

            try
            {
                string query = @"UPDATE sms.std_registration
                           SET std_first_name = @std_first_name
                              ,std_last_name = @std_last_name
                              ,std_father_name = @std_father_name
                              ,std_mother_name = @std_mother_name
                              ,std_address = @std_address
                              ,std_address1 = @std_address1
                              ,std_address2 = @std_address2
                              ,std_district = @std_district
                              ,std_state = @std_state
                              ,std_country = @std_country
                              ,std_pincode = @std_pincode
                              ,std_contact = @std_contact
                              ,std_contact1 = @std_contact1
                              ,std_contact2 = @std_contact2
                              ,std_email = @std_email
                              ,std_class_id = @std_class_id
		                        WHERE
		                        session = @session
		                        and
		                        reg_no = @reg_no
		                        and
		                        reg_date = @reg_date";

                con.Execute(query, std);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public std_registration DeleteRegistration(String sess, int reg, DateTime reg_dt)
        {
            String Query = "DELETE FROM sms.std_registration WHERE session = @session and reg_no = @reg_no and reg_date = @reg_date";

            return con.Query<std_registration>(Query, new { session = sess, reg_no = reg, reg_date = reg_dt }).SingleOrDefault();
        }
    }
}