using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Dapper;

namespace SMS.Models
{
   
    public class emp_detailMain
    {
        

        public void AddEmployee(emp_detail emp)
        {

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string maxid = "select ifnull(MAX(user_id),0)+1 from emp_profile";

                    int id = con.ExecuteScalar<int>(maxid);



                    string query = @"INSERT INTO `emp_profile`
                                    (`user_id`,
                                    `FirstName`,
                                    `LastName`,
                                    `Email`,
                                    `contact`,
                                    `contact1`,
                                    `contact2`,
                                    `address`,
                                    `address1`,
                                    `address2`,
                                    `district`,
                                    `state`,
                                    `country`,
                                    `pincode`,
                                    `FatherName`,
                                    `MotherName`,
                                    `dob`,
                                    `sex`,
                                    `education`,
                                    `doj`,
                                    `epf_no`,
                                    `aadhaar_no`,
                                    `pan_no`,
                                    `bank_name`,
                                    `acc_no`,
                                    `ifsc_code`,
                                    `bank_branch`,
                                    `designation`,
                                    `bioMatricNo`,
                                     emp_active)
                                    VALUES
                                    (@user_id,
                                    @First_Name,
                                    @Last_Name ,
                                    @email,
                                    @contact,
                                    @contact1,
                                    @contact2,
                                    @address,
                                    @address1,
                                    @address2,
                                    @district,
                                    @state,
                                    @country,
                                    @pincode,
                                    @FatherName,
                                    @MotherName,
                                    @dob,
                                    @sex,
                                    @education,
                                    @doj,
                                    @epf_no,
                                    @aadhaar_no,
                                    @pan_no,
                                    @bank_name,
                                    @acc_no,
                                    @ifsc_no,
                                    @bank_branch,
                                    @designation,
                                    @bioMatricNo,
                                        1)";

                    emp.user_id = id;

                    con.Execute(query,
                            new
                            {
                                emp.user_id,
                                emp.first_name,
                                emp.last_name,
                                emp.email,
                                emp.contact,
                                emp.contact1,
                                emp.contact2,
                                emp.address,
                                emp.address1,
                                emp.address2,
                                emp.district,
                                emp.state,
                                emp.country,
                                emp.pincode,
                                emp.FatherName,
                                emp.MotherName,
                                emp.dob,
                                emp.sex,
                                emp.education,
                                emp.doj,
                                emp.epf_no,
                                emp.aadhaar_no,
                                emp.pan_no,
                                emp.bank_name,
                                emp.acc_no,
                                emp.ifsc_no,
                                emp.bank_branch,
                                emp.designation,
                                emp.bioMatricNo

                            });


                }

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<emp_detail> DDFacultyList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT user_id,concat(ifnull(FirstName,''),' ',ifnull(LastName,'')) user_name FROM emp_profile where emp_active = 1";

                var result = con.Query<emp_detail>(query);

                return result;
            }
        }


        public IEnumerable<emp_detail> AllEmpList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String query = @"select user_id,FirstName first_name,LastName last_name,Email,contact from emp_profile where emp_active = 1";

                var result = con.Query<emp_detail>(query);

                return result;
            }
        }

        public emp_detail FindEmployee(int user_id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String query = @"SELECT `emp_profile`.`user_id`,
                                `emp_profile`.`FirstName` first_name,
                                `emp_profile`.`LastName` last_name,
                                `emp_profile`.`Email`,
                                `emp_profile`.`contact`,
                                `emp_profile`.`contact1`,
                                `emp_profile`.`contact2`,
                                `emp_profile`.`address`,
                                `emp_profile`.`address1`,
                                `emp_profile`.`address2`,
                                `emp_profile`.`district`,
                                `emp_profile`.`state`,
                                `emp_profile`.`country`,
                                `emp_profile`.`pincode`,
                                `emp_profile`.`FatherName`,
                                `emp_profile`.`MotherName`,
                                `emp_profile`.`dob`,
                                `emp_profile`.`sex`,
                                `emp_profile`.`education`,
                                `emp_profile`.`doj`,
                                `emp_profile`.`epf_no`,
                                `emp_profile`.`aadhaar_no`,
                                `emp_profile`.`pan_no`,
                                `emp_profile`.`bank_name`,
                                `emp_profile`.`acc_no`,
                                `emp_profile`.`ifsc_code` ifsc_no,
                                `emp_profile`.`bank_branch`,
                                `emp_profile`.`designation`,
                                `emp_profile`.`bioMatricNo`,
                                `emp_profile`.`emp_active`
                            FROM `emp_profile`
                            where user_id = @user_id";

                var result = con.Query<emp_detail>(query, new { user_id = user_id }).SingleOrDefault();

                return result;

            }
        }

        public void EditEmp(emp_detail emp)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"UPDATE `emp_profile`
                                SET
                                `FirstName` = @first_Name,
                                `LastName` = @last_Name,
                                `Email` = @email,
                                `contact` = @contact,
                                `contact1` = @contact1,
                                `contact2` = @contact2,
                                `address` = @address,
                                `address1` = @address1,
                                `address2` = @address2,
                                `district` = @district,
                                `state` = @state,
                                `country` = @country,
                                `pincode` = @pincode,
                                `FatherName` = @FatherName,
                                `MotherName` = @MotherName,
                                `dob` = @dob,
                                `sex` = @sex,
                                `education` = @education,
                                `doj` = @doj,
                                `epf_no` = @epf_no,
                                `aadhaar_no` = @aadhaar_no,
                                `pan_no` = @pan_no,
                                `bank_name` = @bank_name,
                                `acc_no` = @acc_no,
                                `ifsc_code` = @ifsc_no,
                                `bank_branch` = @bank_branch,
                                `designation` = @designation,
                                `bioMatricNo` = @bioMatricNo,
                                `emp_active` = @emp_active
                                WHERE `user_id` = @user_id";

                con.Execute(query, emp);

                if(!emp.emp_active)
                {
                    query = @"DELETE FROM `enable_features` 
                                WHERE
                                    user_id = @user_id;";

                    con.Execute(query, emp);

                    query = @"DELETE FROM `enable_wedget` 
                                WHERE
                                    user_id = @user_id;";

                    con.Execute(query, emp);

                    query = @"DELETE FROM `mst_attendance` 
                                WHERE
                                    user_id = @user_id;";

                    con.Execute(query, emp);

                    query = @"DELETE FROM `hariti`.`users` 
                                WHERE
                                    user_id = @user_id;";

                    con.Execute(query, emp);
                }

            }
        }
    }
}