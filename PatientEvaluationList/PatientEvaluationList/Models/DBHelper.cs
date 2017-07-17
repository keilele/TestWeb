using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Collections;
using System.Data;

namespace PatientEvaluationList
{
    /// <summary>
    /// 数据处理类
    /// </summary>
    public class DBHelper
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        public DBHelper(string connectionString)
        {
            ConnectionString = connectionString;
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString
        { get; private set; }


        /// <summary>
        /// 查询数据，返回List
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="pars">参数</param>
        /// <returns></returns>
        public List<Dictionary<string, dynamic>> GetList(string sql, params SqlParameter[] pars)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(pars);
                con.Open();
                var dr = cmd.ExecuteReader();
                var list = new List<Dictionary<string, dynamic>>();
                //读取list数据
                while (dr.Read())
                {
                    var newdic = new Dictionary<string, dynamic>();
                    foreach (var dicitem in Enumerable.Range(0, dr.FieldCount).ToDictionary(dr.GetName, dr.GetValue))
                    {
                        newdic.Add(dicitem.Key, dicitem.Value == DBNull.Value ? "" : dicitem.Value);
                    }
                    list.Add(newdic);
                }
                return list;
            }
        }

        /// <summary>
        /// 查询数据，返回单个值
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="pars">参数</param>
        /// <returns></returns>
        public object GetValue(string sql, params SqlParameter[] pars)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(pars);
                con.Open();
                var value = cmd.ExecuteScalar();
                return value;
            }
        }

        /// <summary>
        /// 执行存储过程,返回list
        /// </summary>
        /// <param name="procName">存储过程名字</param>
        /// <param name="pars">参数列表</param>
        /// <returns></returns>
        public List<Dictionary<string, dynamic>> ExecProcedureBackList(string procName, params SqlParameter[] pars)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = procName;
                cmd.Parameters.AddRange(pars);
                con.Open();
                var dr = cmd.ExecuteReader();
                var list = new List<Dictionary<string, dynamic>>();
                //读取list数据
                while (dr.Read())
                {
                    list.Add(Enumerable.Range(0, dr.FieldCount).ToDictionary(dr.GetName, dr.GetValue));
                }
                return list;
            }
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名字</param>
        /// <param name="pars">参数列表</param>
        /// <returns></returns>
        public int ExecProcedure(string procName, params SqlParameter[] pars)
        {

            using (var con = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = procName;
                cmd.Parameters.AddRange(pars);
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// 增删改数据
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="pars">参数</param>
        /// <returns></returns>
        public int SavaData(string sql, params SqlParameter[] pars)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                var cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = sql;
                cmd.Parameters.AddRange(pars);
                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 事务中增删改数据
        /// </summary>
        /// <param name="sqls">sql语句列表</param>
        /// <param name="parsList">参数列表</param>
        /// <returns></returns>
        public bool SavaDataTransaction(List<string> sqls, List<List<SqlParameter>> parsList)
        {
            using (var con = new SqlConnection(ConnectionString))
            {
                con.Open();
                var tran = con.BeginTransaction();
                try
                {
                    var cmd = new SqlCommand();
                    cmd.Transaction = tran;
                    cmd.Connection = con;
                    for (int i = 0; i < sqls.Count; i++)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = sqls[i];
                        cmd.Parameters.AddRange(parsList[i].ToArray());
                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                    return true;
                }
                catch (Exception)
                {
                    tran.Rollback();
                    return false;
                }
                finally
                {
                    con.Close();
                }
            }

        }

        /// <summary>
        /// 获取交易订单号
        /// </summary>
        /// <param name="orderNo">订单编码</param>
        /// <returns></returns>
        public string GetOrderNumber(string orderNo)
        {

            using (var con = new SqlConnection(ConnectionString))
            {

                var cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "GetTradeNumber"; 
                cmd.CommandType = CommandType.StoredProcedure;
                var tradeNumber = "DD"+orderNo.PadLeft(7, '0');//000000001
                cmd.Parameters.Add(new SqlParameter("@tradeNumber", tradeNumber));

                var outPar = new SqlParameter();
                outPar.ParameterName = "@newTradeNumber";
                outPar.SqlDbType = SqlDbType.NVarChar;
                outPar.Size = 30;
                outPar.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outPar);
                con.Open();

                cmd.ExecuteNonQuery();
                return outPar.Value.ToString();


            }
        }

        /// <summary>
        /// 执行存储过程生成时间列表
        /// </summary>
        /// <param name="begTime">起始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="limitTime">间隔</param>
        /// <param name="doctid">医生ID</param>
        /// <param name="selectDate">日期</param>
        /// <returns></returns>
        public bool SearchTimeDepart(DateTime begTime, DateTime endTime, int limitTime, string doctid,DateTime selectDate)
        {
            try
            {
                using (var con = new SqlConnection(ConnectionString))
                {

                    var cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "SearchTimeDepart";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@begintime", begTime));
                    cmd.Parameters.Add(new SqlParameter("@endtime", endTime));
                    cmd.Parameters.Add(new SqlParameter("@doctorid", doctid));
                    cmd.Parameters.Add(new SqlParameter("@period", limitTime));
                    cmd.Parameters.Add(new SqlParameter("@selectDate", selectDate));
                    con.Open();

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

          
        }

    }
}
