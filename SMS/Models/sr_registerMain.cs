using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;
using SMS.report;

namespace SMS.Models
{
    public class sr_registerMain
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddStudent(sr_register std)
        {

            try
            {
                mst_fin fin = new mst_fin();

                string query1 = @"SELECT [fin_id]
                              ,[fin_start_date]
                              ,[fin_end_date]
                              ,[fin_close]
                          FROM [SMS].[dbo].[mst_fin]
                          where fin_close = 'N'";

                fin = con.Query<mst_fin>(query1).SingleOrDefault();
                if (DateTime.Parse(std.std_admission_date_str) > fin.fin_start_date && DateTime.Parse(std.std_admission_date_str) < fin.fin_end_date)
                {

                    string batch_query = "select batch_id from mst_batch where class_id = @class_id";

                    int batch_no = con.Query<int>(batch_query, new { class_id = std.class_id }).SingleOrDefault();

                    string maxid = "select isnull(MAX(sr_number),0)+1 from sr_register";

                    //                var id = con.Query<mst_section>(maxid).ToString().Trim();

                    int id = con.ExecuteScalar<int>(maxid);



                    string query = @"INSERT INTO [dbo].[sr_register]
           ([sr_number]
           ,[std_first_name]
           ,[std_last_name]
           ,[std_father_name]
           ,[std_mother_name]
           ,[std_address]
           ,[std_address1]
           ,[std_address2]
           ,[std_district]
           ,[std_state]
           ,[std_country]
           ,[std_pincode]
           ,[std_contact]
           ,[std_contact1]
           ,[std_contact2]
           ,[std_email]
           ,[std_father_occupation]
           ,[std_mother_occupation]
           ,[std_blood_gp]
           ,[std_house_income]
           ,[std_nationality]
           ,[std_category]
           ,[std_cast]
           ,[std_dob]
           ,[std_sex]
           ,[std_last_school]
           ,[std_admission_date]
           ,[std_section_id]
           ,[std_batch_id]
           ,[std_house]
           ,[std_remark]
           ,[std_active]
           ,[std_pickup_id]
           ,[std_admission_class]
           ,[adm_session]
           ,[reg_no]
           ,[reg_date])
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
           ,@reg_date)";

                    std.std_batch_id = batch_no;
                    std.std_active = "Y";
                    std.sr_number = id;
                    std.std_admission_date = DateTime.Parse(std.std_admission_date_str);
                    std.std_dob = DateTime.Parse(std.std_dob_str);
                    con.Execute(query,
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

                            });

                    std_registrationMain main = new std_registrationMain();

                    main.DeleteRegistration(std.adm_session, std.reg_no, std.reg_date);

                    fees_receiptMain mstfees = new fees_receiptMain();
                    fees_receipt fees = new fees_receipt();

                    fees.sr_number = std.sr_number;
                    fees.class_id = std.class_id;
                    fees.section_id = std.std_section_id;
                    fees.batch_id = std.std_batch_id;

                    fees.reg_no = std.reg_no;

                    fees.reg_date = std.reg_date;

                    mstfees.updateReceipt(fees);

                    /* mst_acc_head head = new mst_acc_head();

                     mst_acc_headMain headMain = new mst_acc_headMain();

                     head = headMain.FindAccount(8);


                     fees_receiptMain mstfees = new fees_receiptMain();
                     fees_receipt fees = new fees_receipt();


                     fees.amount = std.fees_amount;
                     fees.acc_id = 8;
                     fees.fees_name = head.acc_name;
                     fees.reg_no = std.reg_no;
                     fees.reg_date = std.reg_date;
                     fees.fin_id = fin.fin_id;
                     fees.class_id = std.class_id;
                     fees.nature = head.nature;
                     fees.amount = std.fees_amount;
                     fees.sr_number = std.sr_number;
                     fees.section_id = std.std_section_id;
                     fees.batch_id = std.std_batch_id;

                     mstfees.updateReceipt(fees);

                     mstfees.AddReceipt(fees);

                     repFees_receipt pdf = new repFees_receipt();

                     pdf.pdf(fees.fin_id, fees.receipt_no, fees.receipt_date.ToShortDateString());*/

                    out_standing out_std = new out_standing();

                    out_std.acc_id = 2;
                    out_std.outstd_amount = std.fees_amount;
                    //out_std.reg_num = std.reg_no;

                    out_standingMain out_stdMain = new out_standingMain();

                    out_std.sr_number = std.sr_number;

                    out_stdMain.AddOutStanding(out_std);

                    out_std.reg_num = std.reg_no;

                    out_std.dt_date = std.reg_date;

                    out_stdMain.updateOutstanding(out_std);

                    var p = new DynamicParameters();

                    p.Add("@sr_num", std.sr_number);

                    con.Execute("[dbo].[monthlyFees]", p, commandType: System.Data.CommandType.StoredProcedure);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       
        public IEnumerable<sr_register> AllStudentList()
        {
            String query = @"select sr_number,std_first_name,std_last_name,std_father_name,c.class_name,b.section_name,std_active 
                                from [dbo].[sr_register] a, [dbo].[mst_section] b,[dbo].[mst_class] c
                                where
                                a.std_section_id = b.section_id
                                and
                                b.class_id = c.class_id";

            var result = con.Query<sr_register>(query);

            return result;
        }

        public sr_register FindStudent(int? id)
        {
            String Query = @"SELECT [sr_number]
                              ,[std_first_name]
                              ,[std_last_name]
                              ,[std_father_name]
                              ,[std_mother_name]
                              ,[std_address]
                              ,[std_address1]
                              ,[std_address2]
                              ,[std_district]
                              ,[std_state]
                              ,[std_country]
                              ,[std_pincode]
                              ,[std_contact]
                              ,[std_contact1]
                              ,[std_contact2]
                              ,[std_email]
                              ,[std_father_occupation]
                              ,[std_mother_occupation]
							  ,[std_blood_gp]
							  ,[std_house_income]
							  ,[std_nationality]
							  ,[std_category]
                              ,[std_cast]
                              ,[std_dob]
							  ,[std_sex]
                              ,[std_last_school]
                              ,[std_admission_date]
                              ,[std_admission_class]
                              ,c.class_name
                              ,c.class_id
                              ,[std_section_id]
                              ,b.[section_name]
                              ,[std_batch_id]
                              ,[std_house]
                              ,[std_remark]
                              ,[std_pickup_id]
                              ,d.[pickup_point]
							  ,[adm_session]
							  ,[reg_no]
							  ,[reg_date]
                              ,[std_active]
                             FROM[dbo].[sr_register] a, [dbo].[mst_section] b,[dbo].[mst_class] c,[dbo].[mst_transport] d
							  where
							 a.std_section_id = b.section_id
							 and
							 b.class_id = c.class_id 
							 and
							 isnull(a.std_pickup_id,0) = d.pickup_id
                             and
							 a.sr_number = @sr_number";
          
            return con.Query<sr_register>(Query, new { sr_number = id }).SingleOrDefault();
        }

        public void EditStudent(sr_register std)
        {
            
            try
            {
                string query = @"UPDATE [dbo].[sr_register]
   SET [std_first_name] = @std_first_name
      ,[std_last_name] = @std_last_name
      ,[std_father_name] = @std_father_name
      ,[std_mother_name] = @std_mother_name
      ,[std_address] = @std_address
      ,[std_address1] = @std_address1
      ,[std_address2] = @std_address2
      ,[std_district] = @std_district
      ,[std_state] = @std_state
      ,[std_country] = @std_country
      ,[std_pincode] = @std_pincode
      ,[std_contact] = @std_contact
      ,[std_contact1] = @std_contact1
      ,[std_contact2] = @std_contact2
      ,[std_email] = @std_email
      ,[std_father_occupation] = @std_father_occupation
      ,[std_mother_occupation] = @std_mother_occupation
      ,[std_blood_gp] = @std_blood_gp
      ,[std_house_income] = @std_house_income
      ,[std_nationality] = @std_nationality
      ,[std_category] = @std_category
      ,[std_cast] = @std_cast
      ,[std_dob] = @std_dob
      ,[std_sex] = @std_sex
      ,[std_last_school] = @std_last_school
      ,[std_admission_date] = @std_admission_date
      ,[std_section_id] = @std_section_id
      ,[std_batch_id] = @std_batch_id
      ,[std_house] = @std_house
      ,[std_remark] = @std_remark
      ,[std_active] = @std_active
      ,[std_pickup_id] = @std_pickup_id
      ,[std_admission_class] = @std_admission_class
        WHERE sr_number = @sr_number"; 

                con.Execute(query,std);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public sr_register DeleteStudent(int id)
        {
            String Query = "DELETE FROM [dbo].[sr_register] WHERE sr_number = @sr_number";

            return con.Query<sr_register>(Query, new { sr_number = id }).SingleOrDefault();
        }

    }
}