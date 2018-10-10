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

                
                if (sess.checkSessionNotExpired())
                {

                    string batch_query = "select batch_id from mst_batch where class_id = @class_id";

                    int batch_no = con.Query<int>(batch_query, new { class_id = std.class_id }).SingleOrDefault();

                    string maxid = "select ifnull(MAX(sr_number),0)+1 from sr_register";

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
           ,std_section_id
           ,std_batch_id
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
           ,@std_section_id
           ,@std_batch_id
           ,@std_house
           ,@std_remark
           ,@std_active
           ,@std_pickup_id
           ,@std_admission_class
           ,@adm_session
           ,@reg_no
           ,@reg_date
           ,@std_aadhar)";

                    std.std_batch_id = batch_no;
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
                                std.std_section_id
                               ,
                                std.std_batch_id
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

                    std_registrationMain main = new std_registrationMain();

                    main.DeleteRegistrationOnly(std.adm_session, std.reg_no, std.reg_date);

                    fees_receiptMain mstfees = new fees_receiptMain();
                    fees_receipt fees = new fees_receipt();

                    fees.sr_number = std.sr_number;
                    fees.class_id = std.class_id;
                    fees.section_id = std.std_section_id;
                    fees.batch_id = std.std_batch_id;

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

                    out_std.reg_num = std.reg_no;

                    out_std.dt_date = std.reg_date;

                    out_std.class_id = std.class_id;

                    out_stdMain.updateOutstanding(out_std);

                    var p = new DynamicParameters();

                    SMSMessage sms = new SMSMessage();

                    foreach (var item in sms.smsbody("admission"))
                    {
                        string body = item.Replace("#student_name#", std.std_first_name + " " + std.std_last_name);

                        body = body.Replace("#class#", std.std_admission_class);

                        body = body.Replace("#sr_number#", std.sr_number.ToString());

                        await sms.SendSMS(body, std.std_contact,true);
                    }


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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        public IEnumerable<sr_register> AllStudentList(int section_id)
        {
            string query = @"select sr_number,std_first_name,std_last_name,std_father_name,c.class_name,b.section_name,coalesce(a.std_contact,a.std_contact1,a.std_contact2) std_contact
                                from sr_register a, mst_section b,mst_class c
                                where
                                a.std_section_id = b.section_id
                                and
                                b.class_id = c.class_id
                                and
                                b.section_id = @section_id
                                and a.std_active = 'Y'";

            var result = con.Query<sr_register>(query,new { section_id = section_id });

            return result;
        }

        //public IEnumerable<sr_register> AllNSOStudentList(int section_id)
        //{
        //    String query = @"select sr_number,std_first_name,std_last_name,std_father_name,c.class_name,b.section_name,coalesce(a.std_contact,a.std_contact1,a.std_contact2) std_contact
        //                        from sr_register a, mst_section b,mst_class c
        //                        where
        //                        a.std_section_id = b.section_id
        //                        and
        //                        b.class_id = c.class_id
        //                        and
        //                        b.section_id = @section_id
        //                        and a.std_active = 'N'";

        //    var result = con.Query<sr_register>(query, new { section_id = section_id });

        //    return result;
        //}

        public sr_register FindStudent(int? id)
        {
            String Query = @"SELECT sr_number
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
                              ,std_admission_class
                              ,c.class_name
                              ,c.class_id
                              ,std_section_id
                              ,b.section_name
                              ,std_batch_id
                              ,std_house
                              ,std_remark
                              ,std_pickup_id
                              ,d.pickup_point
							  ,adm_session
							  ,reg_no
							  ,reg_date
                              ,std_active
                              ,std_aadhar
                              ,nso_date
                             FROM sr_register a, mst_section b,mst_class c,mst_transport d
							  where
							 a.std_section_id = b.section_id
							 and
							 b.class_id = c.class_id 
							 and
							 ifnull(a.std_pickup_id,0) = d.pickup_id
                             and
							 a.sr_number = @sr_number";
          
            return con.Query<sr_register>(Query, new { sr_number = id }).SingleOrDefault();
        }

        public void EditStudent(sr_register std)
        {
            
            try
            {
                string query1 = @"select std_pickup_id from sr_register where sr_number = @sr_number";

                int pick_id = con.Query<int>(query1, new { sr_number = std.sr_number }).SingleOrDefault();

                query1 = @"select std_batch_id from sr_register where sr_number = @sr_number";

                int batch = con.Query<int>(query1, new { sr_number = std.sr_number }).SingleOrDefault();

                query1 = @"select batch_id from mst_batch where class_id = @class_id";

                std.std_batch_id = con.Query<int>(query1, new { class_id = std.class_id }).SingleOrDefault();

                query1 = @"select std_section_id from sr_register where sr_number = @sr_number";

                int sec_id = con.Query<int>(query1, new { sr_number = std.sr_number }).SingleOrDefault();




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
      ,std_section_id = @std_section_id
      ,std_batch_id = @std_batch_id
      ,std_house = @std_house
      ,std_remark = @std_remark
      ,std_active = @std_active
      ,std_pickup_id = @std_pickup_id
      ,std_admission_class = @std_admission_class
      ,std_aadhar = @std_aadhar
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

                        con.Execute("StdMidSessionTransportChange", p, commandType: System.Data.CommandType.StoredProcedure);

                        DashboardHub hub = new DashboardHub();

                        hub.DashboardSchoolStrength();

                    }

                    if (batch != std.std_batch_id)
                    {
                        //call procedure to change the class

                        var p = new DynamicParameters();

                        p.Add("@sr_num", std.sr_number);

                        con.Execute("stdMidSessionMonthlyCharge", p, commandType: System.Data.CommandType.StoredProcedure);


                    }

                    if (sec_id != std.std_section_id)
                    {
                        mst_sessionMain sess = new mst_sessionMain();

                        string session = sess.findActive_finalSession();

                        query = @"update attendance_register set section_id = @section_id where `session` = @session AND `user_id` != 0  AND `att_date` != '2001-01-01' AND `class_id` = @class_id AND `section_id` = @sect_id AND `sr_num` = @sr_num AND `roll_no` != 0";

                        con.Execute(query, new { section_id = std.std_section_id,sect_id=sec_id ,sr_num = std.sr_number, session= session, class_id = std.class_id });

                        query = @"DELETE FROM `mst_rollnumber`
                                    WHERE session = @session
                                    and
                                    sr_num = @sr_num
                                    and
                                    class_id = @class_id
                                    and
                                    section_id = @section_id
                                    and
                                    roll_number != 0 ";

                        con.Execute(query, new { section_id = sec_id, sr_num = std.sr_number, session = session,class_id = std.class_id });
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