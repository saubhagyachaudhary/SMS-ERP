﻿using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_staffMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public IEnumerable<mst_staff> mentor_list()
        {

            string query = @"SELECT 
                                    user_id staff_id,
                                    CONCAT(FirstName, ' ', LastName) staff_name
                                FROM
                                    emp_profile
                                WHERE
                                    emp_active = 1";

            var result = con.Query<mst_staff>(query);

            return result;
        }
    }
}