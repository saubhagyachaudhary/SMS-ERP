﻿using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace SMS.Models
{
    public class mst_sectionMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddSection(mst_section mst)
        {
            try
            {
                mst_sessionMain sess = new mst_sessionMain();

                string session = sess.findActive_finalSession();

                string duplicate = "select count(*) from mst_section where class_id = @class_id and section_name = @section_name";

                int dup = con.ExecuteScalar<int>(duplicate,new { mst.class_id,mst.Section_name});       

                if (dup > 0)
                {
                    throw new DuplicateWaitObjectException();
                }
                else
                {
                    string query = "INSERT INTO mst_section(section_id,class_id,section_name,session) VALUES (@section_id,@class_id,@section_name,@session)";

                    string maxid = "select ifnull(MAX(section_id),0)+1 from mst_section where session=@session";

                    //                var id = con.Query<mst_section>(maxid).ToString().Trim();

                    int id = con.ExecuteScalar<int>(maxid,new { session = session });

                    if (id == 1)
                    {
                        id = 100;
                    }

                    mst.section_id = id;
                    mst.Section_name = mst.Section_name.Trim();

                    con.Execute(query, new
                    {
                        mst.section_id,
                        mst.class_id,
                        mst.Section_name,
                        session = session
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_section> AllSectionList()
        {
            String query = "SELECT section.section_id,class.class_name,section_name FROM mst_section section,mst_class class where class.class_id = section.class_id order by class.class_id ";

            var result = con.Query<mst_section>(query);

            return result;
        }

        public mst_section FindSection(int? id)
        {
            String Query = "SELECT section.section_id,class.class_name,section_name FROM mst_section section,mst_class class where class.class_id = section.class_id and section.section_id = @section_id";

            return con.Query<mst_section>(Query, new { section_id = id }).SingleOrDefault();
        }

        public void EditSection(mst_section mst)
        {

            try
            {
                string query = "UPDATE mst_section SET section_id = @section_id ,class_id = @class_id,section_name = @section_name WHERE section_id = @section_id";

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_section DeleteSection(int id)
        {
            String Query = "DELETE FROM mst_section WHERE section_id = @section_id";

            return con.Query<mst_section>(Query, new { section_id = id }).SingleOrDefault();
        }
    }
}