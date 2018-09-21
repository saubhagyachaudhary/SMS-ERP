using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_disciplineMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddDiscipline(mst_discipline mst)
        {
            try
            {
                mst_sessionMain session = new mst_sessionMain();

                string query = "INSERT INTO mst_discipline (session,discipline_id,discipline_name) VALUES (@session,@discipline_id,@discipline_name)";

                string maxid = "select ifnull(MAX(discipline_id),0)+1 from mst_discipline";

                int id = con.ExecuteScalar<int>(maxid);

                mst.session = session.findActive_finalSession();
                mst.discipline_id = id;
                mst.discipline_name = mst.discipline_name.Trim();

                con.Execute(query, new
                {
                    mst.session,
                    mst.discipline_id,
                    mst.discipline_name
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_discipline> AllDisciplineList()
        {
            mst_sessionMain session = new mst_sessionMain();

            string query = "SELECT discipline_id,discipline_name FROM mst_discipline where session = @session";

            var result = con.Query<mst_discipline>(query,new {session = session.findActive_finalSession() });

            return result;
        }


        public mst_discipline FindDiscipline(int? id)
        {
            mst_sessionMain session = new mst_sessionMain();

            string Query = "SELECT discipline_id,discipline_name FROM mst_discipline where discipline_id = @discipline_id and session = @session";

            return con.Query<mst_discipline>(Query, new { discipline_id = id,session = session.findActive_finalSession() }).SingleOrDefault();
        }

        public void EditDiscipline(mst_discipline mst)
        {

            try
            {
                mst_sessionMain session = new mst_sessionMain();

                string query = "UPDATE mst_discipline SET discipline_name = @discipline_name WHERE discipline_id = @discipline_id and session = @session";

                mst.session = session.findActive_finalSession();

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_discipline DeleteDiscipline(int id)
        {
            try
            {
                mst_sessionMain session = new mst_sessionMain();

                string Query = "DELETE FROM mst_discipline WHERE discipline_id = @discipline_id and session = @session";

                return con.Query<mst_discipline>(Query, new { discipline_id = id,session = session.findActive_finalSession() }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}