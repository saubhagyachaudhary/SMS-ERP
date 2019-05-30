using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using MySql.Data.MySqlClient;
using System.Configuration;
using SMS.report;
using System.Threading.Tasks;
using SMS.Hubs;

namespace SMS.Models
{
    public class sr_registerMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public async Task AddStudent(sr_register std)
        {

            try
            {
                mst_sessionMain sess = new mst_sessionMain();

               

                   

                    string maxid = @"SELECT 
                                            IFNULL(MAX(sr_number), 0) + 1
                                        FROM
                                            sr_register";

                    //                var id = con.Query<mst_section>(maxid).ToString().Trim();

                    int id = con.ExecuteScalar<int>(maxid);



                    string query = @"INSERT INTO sr_register
                                   (sr_number
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
                                   ,std_father_occupation
                                   ,std_mother_occupation
                                   ,std_blood_gp
                                   ,std_house_income
                                   ,std_nationality
                                   ,std_category
                                   ,std_cast
                                   ,std_dob
                                   ,std_sex
                                   ,std_last_school
                                   ,std_admission_date
                                   ,std_house
                                   ,std_remark
                                   ,std_active
                                   ,std_pickup_id
                                   ,std_admission_class
                                   ,adm_session
                                   ,reg_no
                                   ,reg_date
                                   ,std_aadhar)
                             VALUES
                                   (@sr_number
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
                                   ,@std_father_occupation
                                   ,@std_mother_occupation
                                   ,@std_blood_gp
                                   ,@std_house_income
                                   ,@std_nationality
                                   ,@std_category
                                   ,@std_cast
                                   ,@std_dob
                                   ,@std_sex
                                   ,@std_last_school
                                   ,@std_admission_date
                                   ,@std_house
                                   ,@std_remark
                                   ,@std_active
                                   ,@std_pickup_id
                                   ,@std_admission_class
                                   ,@adm_session
                                   ,@reg_no
                                   ,@reg_date
                                   ,@std_aadhar)";



                   
                    std.std_active = "Y";
                    std.sr_number = id;
                    std.std_admission_date = DateTime.Parse(std.std_admission_date_str);
                    std.std_dob = DateTime.Parse(std.std_dob_str);
                    await con.ExecuteAsync(query,
                            new
                            {
                                std.sr_number
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
                                std.std_father_occupation
                               ,
                                std.std_mother_occupation
                               ,
                                std.std_blood_gp
                               ,
                                std.std_house_income
                               ,
                                std.std_nationality
                               ,
                                std.std_category
                               ,
                                std.std_cast
                               ,
                                std.std_dob
                               ,
                                std.std_sex
                               ,
                                std.std_last_school
                               ,
                                std.std_admission_date
                               ,
                                std.std_admission_class
                               ,
                                std.std_house
                               ,
                                std.std_remark
                               ,
                                std.std_pickup_id
                               ,
                                std.std_active
                               ,
                                std.adm_session
                               ,
                                std.reg_no
                               ,
                                std.reg_date
                                ,
                                std.std_aadhar

                            });

                  query = @"INSERT INTO `mst_std_class`
                            (`session`,
                            `sr_num`,
                            `class_id`)
                            VALUES
                            (@session,
                            @sr_num,
                            @class_id)";

                    

                    await con.ExecuteAsync(query,new{session = sess.findActive_Session() ,sr_num = std.sr_number, class_id = std.class_id});

                    query = @"INSERT INTO `mst_std_section`
                            (`session`,
                            `sr_num`,
                            `section_id`)
                            VALUES
                            (@session,
                            @sr_num,
                            @section_id)";



                    await con.ExecuteAsync(query, new { session = sess.findActive_Session(), sr_num = std.sr_number, section_id = std.std_section_id });

                    std_registrationMain main = new std_registrationMain();

                    main.DeleteRegistrationOnly(std.adm_session, std.reg_no, std.reg_date);

                    fees_receiptMain mstfees = new fees_receiptMain();
                    fees_receipt fees = new fees_receipt();

                    fees.sr_number = std.sr_number;
                    fees.class_id = std.class_id;
                    fees.section_id = std.std_section_id;

                    fees.reg_no = std.reg_no;

                    fees.reg_date = std.reg_date;

                    mstfees.updateReceipt(fees);

                    out_standing out_std = new out_standing();

                    out_std.acc_id = 2;
                    out_std.outstd_amount = std.fees_amount;
                   

                    out_standingMain out_stdMain = new out_standingMain();

                    out_std.sr_number = std.sr_number;
                    out_std.class_id = std.class_id;

                    out_stdMain.AddOutStanding(out_std);

                query = @"SELECT 
                                acc_id, fees_amount outstd_amount
                            FROM
                                mst_fees
                            WHERE
                                session = @session AND bl_onetime = 1
                                    AND acc_id != 2
                                    AND class_id = @class_id";

                out_standing out_std_onetime = new out_standing();

                out_std_onetime = con.Query<out_standing>(query, new {session = sess.findActive_Session(), class_id = std.class_id }).SingleOrDefault();

                if(out_std_onetime != null)
                {
                    out_std_onetime.sr_number = std.sr_number;

                    out_std_onetime.class_id = std.class_id;

                    out_stdMain.AddOutStanding(out_std_onetime);
                }
                
                out_std.reg_num = std.reg_no;

                    out_std.dt_date = std.reg_date;

                    out_std.class_id = std.class_id;

                    out_stdMain.updateOutstanding(out_std);

                    var p = new DynamicParameters();
#if !DEBUG
                SMSMessage sms = new SMSMessage();

                    foreach (var item in sms.smsbody("admission"))
                    {
                        string body = item.Replace("#student_name#", std.std_first_name + " " + std.std_last_name);

                        body = body.Replace("#class#", std.std_admission_class);

                        body = body.Replace("#sr_number#", std.sr_number.ToString());

                        await sms.SendSMS(body, std.std_contact,true);
                    }
#endif

                    //string text = @"Admission of " + std.std_first_name + " " + std.std_last_name + " is confirmed in class " + std.std_admission_class + " via admission number " + std.sr_number + ". Congratulation for being a part of hariti family. Thank You. Hariti Public School.";

                    //sms.SendSMS(text, std.std_contact);

                    // text =  std.std_first_name + " " + std.std_last_name + " का प्रवेश कक्षा "+std.std_admission_class+" में होना सुनिश्चित हुआ है। जिसका प्रवेश क्रमांक " + std.sr_number + " है। हरिति परिवार से जुड़ने के लिये आपका धन्यवाद। Hariti Public School.";

                    //sms.SendSMS(text, std.std_contact);


                    p.Add("@sr_num", std.sr_number);


                    con.Execute("MonthlyFeesFullYear", p, commandType: System.Data.CommandType.StoredProcedure);

                    con.Execute("MonthlyTransportFullYear", p, commandType: System.Data.CommandType.StoredProcedure);

                    DashboardHub hub = new DashboardHub();

                    hub.DashboardSchoolStrength();

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        public IEnumerable<sr_register> AllStudentList(int section_id)
        {
            string query = @"SELECT 
                                sr_number,
                                std_first_name,
                                std_last_name,
                                std_father_name,
                                c.class_name,
                                b.section_name,
                                COALESCE(a.std_contact,
                                        a.std_contact1,
                                        a.std_contact2) std_contact
                            FROM
                                sr_register a,
                                mst_section b,
                                mst_class c,
                                mst_std_section d
                            WHERE
                                a.sr_number = d.sr_num
                                    AND d.section_id = b.section_id
                                    AND b.class_id = c.class_id
                                    AND b.session = c.session
                                    AND c.session = d.session
                                    AND d.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
                                    AND b.section_id = @section_id
                                    AND a.std_active = 'Y'";

            var result = con.Query<sr_register>(query,new { section_id = section_id });

            return result;
        }

     

        public sr_register FindStudent(int? id,string session)
        {
            string Query = @"SELECT 
                            sr_number,
                            std_first_name,
                            std_last_name,
                            std_father_name,
                            std_mother_name,
                            std_address,
                            std_address1,
                            std_address2,
                            std_district,
                            std_state,
                            std_country,
                            std_pincode,
                            std_contact,
                            std_contact1,
                            std_contact2,
                            std_email,
                            std_father_occupation,
                            std_mother_occupation,
                            std_blood_gp,
                            std_house_income,
                            std_nationality,
                            std_category,
                            std_cast,
                            std_dob,
                            std_sex,
                            std_last_school,
                            std_admission_date,
                            std_admission_class,
                            c.class_name,
                            c.class_id,
                            b.section_id std_section_id,
                            b.section_name,
                            std_house,
                            std_remark,
                            std_pickup_id,
                            d.pickup_point,
                            adm_session,
                            reg_no,
                            reg_date,
                            std_active,
                            std_aadhar,
                            nso_date,
                            adm_form_link
                        FROM
                            sr_register a,
                            mst_section b,
                            mst_class c,
                            mst_transport d,
                            mst_std_section e
                        WHERE
                            a.sr_number = e.sr_num
                                AND e.section_id = b.section_id
                                AND b.class_id = c.class_id
                                AND b.session = c.session
                                AND c.session = d.session
                                AND d.session = e.session
                                AND e.session = @session
                                AND IFNULL(a.std_pickup_id, 0) = d.pickup_id
                                AND a.sr_number = @sr_number";
          
            return con.Query<sr_register>(Query, new { sr_number = id,session = session }).SingleOrDefault();
        }

        public void EditStudent(sr_register std)
        {
            
            try
            {
                mst_sessionMain sess = new mst_sessionMain();

                string session = sess.findActive_finalSession();

                string query1 = @"select std_pickup_id from sr_register where sr_number = @sr_number";

                int pick_id = con.Query<int>(query1, new { sr_number = std.sr_number }).SingleOrDefault();

                query1 = @"select class_id from mst_std_class where sr_num = @sr_number and session = @session";

                int class_id = con.Query<int>(query1, new { sr_number = std.sr_number, session = session }).SingleOrDefault();

                query1 = @"select section_id from mst_std_section where sr_num = @sr_number and session = @session";

                int sec_id = con.Query<int>(query1, new { sr_number = std.sr_number,session = session }).SingleOrDefault();




                string query = @"UPDATE sr_register
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
                                      ,std_father_occupation = @std_father_occupation
                                      ,std_mother_occupation = @std_mother_occupation
                                      ,std_blood_gp = @std_blood_gp
                                      ,std_house_income = @std_house_income
                                      ,std_nationality = @std_nationality
                                      ,std_category = @std_category
                                      ,std_cast = @std_cast
                                      ,std_dob = @std_dob
                                      ,std_sex = @std_sex
                                      ,std_last_school = @std_last_school
                                      ,std_admission_date = @std_admission_date
                                      ,std_house = @std_house
                                      ,std_remark = @std_remark
                                      ,std_active = @std_active
                                      ,std_pickup_id = @std_pickup_id
                                      ,std_admission_class = @std_admission_class
                                      ,std_aadhar = @std_aadhar
                                      ,adm_form_link = @adm_form_link
                                        WHERE sr_number = @sr_number";

                con.Execute(query, std);


                if(!std.active)
                {
                    out_standingMain otsd = new out_standingMain();

                    otsd.markStdNSO(std.sr_number);

                    DashboardHub hub = new DashboardHub();

                    hub.DashboardSchoolStrength();
                }
                 else
                {
                    if (pick_id != std.std_pickup_id)
                    {
                        //call procedure to change the pickup point
                        var p = new DynamicParameters();

                        p.Add("@sr_num", std.sr_number);

                        p.Add("@from_month_no", std.from_month_no);

                        con.Execute("StdMidSessionTransportChange", p, commandType: System.Data.CommandType.StoredProcedure);

                        DashboardHub hub = new DashboardHub();

                        hub.DashboardSchoolStrength();

                    }

                    if (class_id != std.class_id)
                    {
                        //call procedure to change the class

                        query = @"UPDATE `mst_std_class` 
                                    SET 
                                        `class_id` = @class_id
                                    WHERE
                                        `session` = @session
                                            AND `sr_num` = @sr_num";

                        con.Execute(query, new {class_id = std.class_id, session = session ,sr_num = std.sr_number });

                        var p = new DynamicParameters();

                        p.Add("@sr_num", std.sr_number);

                        con.Execute("stdMidSessionMonthlyCharge", p, commandType: System.Data.CommandType.StoredProcedure);


                    }

                    if (sec_id != std.std_section_id)
                    {

                        query = @"UPDATE `mst_std_section` 
                                    SET 
                                        `section_id` = @section_id
                                    WHERE
                                        `session` = @session
                                            AND `sr_num` = @sr_num";

                        con.Execute(query, new { section_id = std.std_section_id, session = session, sr_num = std.sr_number });

                        query = @"DELETE FROM `mst_rollnumber`
                                    WHERE session = @session
                                    and
                                    sr_num = @sr_num";

                        con.Execute(query, new { sr_num = std.sr_number, session = session });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public sr_register DeleteStudent(int id)
        {
            String Query = "DELETE FROM sr_register WHERE sr_number = @sr_number";

            return con.Query<sr_register>(Query, new { sr_number = id }).SingleOrDefault();
        }

    }
}