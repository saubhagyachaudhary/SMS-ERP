using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_term_rulesMain
    {

        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddTermRule(mst_term_rules mst)
        {
            try
            {
                mst_sessionMain sess = new mst_sessionMain();

                string maxid = @"SELECT 
                                        IFNULL(MAX(evaluation_id), 0) + 1
                                    FROM
                                        mst_term_rules
                                    WHERE
                                        session = @session";

                int id = con.Query<int>(maxid,new {session = sess.findFinal_Session() }).SingleOrDefault();

                string query = @"INSERT INTO `mst_term_rules`
                                (`session`,
                                `evaluation_id`,
                                `term_id`,
                                `class_id`,
                                `evaluation_name`,
                                `exam_id1`,
                                `exam_id2`,
                                `rule`)
                                VALUES
                                (@session,
                                 @evaluation_id,
                                @term_id,
                                @class_id,
                                @evaluation_name,
                                @exam_id1,
                                @exam_id2,
                                @rule)";

                mst.session = sess.findFinal_Session();

                mst.evaluation_id = id;

                con.Execute(query, new
                {
                    mst.session,
                    mst.evaluation_id,
                    mst.term_id,
                    mst.class_id,
                    mst.evaluation_name,
                    mst.exam_id1,
                    mst.exam_id2,
                    mst.rule
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_term_rules> AllTermRuleList()
        {

            mst_sessionMain sess = new mst_sessionMain();

            string query = @"SELECT 
                                a.session,
                                a.term_id,
                                a.class_id,
                                c.term_name,
                                b.class_name,
                                evaluation_name,
                                evaluation_id,
                                d.exam_name exam_name1,
                                (SELECT 
                                        exam_name
                                    FROM
                                        mst_exam
                                    WHERE
                                        exam_id = a.exam_id2) exam_name2,
                                a.rule
                            FROM
                                mst_term_rules a,
                                mst_class b,
                                mst_term c,
                                mst_exam d
                            WHERE
                                a.class_id = b.class_id
                                    AND a.term_id = c.term_id
                                    AND a.session = @session
                                    AND a.session = b.session
                                    AND b.session = c.session
                                    AND c.session = d.session
                                    AND a.exam_id1 = d.exam_id";

            var result = con.Query<mst_term_rules>(query, new { session = sess.findFinal_Session() });

            return result;
        }

        public mst_term_rules FindTermRule(int class_id, int term_id, string session, int evaluation_id)
        {
            string Query = @"SELECT 
                                    evaluation_name, session, term_id, evaluation_id, class_id
                                FROM
                                    mst_term_rules
                                WHERE
                                    session = @session
                                        AND class_id = @class_id
                                        AND term_id = @term_id
                                        AND evaluation_id = @evaluation_id";

            return con.Query<mst_term_rules>(Query, new { class_id = class_id, term_id = term_id, session = session, evaluation_id = evaluation_id }).SingleOrDefault();
        }

        public mst_term_rules DeleteTermRule(mst_term_rules rule)
        {
            string Query = @"DELETE FROM `mst_term_rules` 
                                WHERE
                                    `session` = @session
                                    AND `evaluation_id` = @evaluation_id
                                    AND `term_id` = @term_id
                                    AND `class_id` = @class_id";

            return con.Query<mst_term_rules>(Query, rule).SingleOrDefault();
        }

    }
}