using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace PatientEvaluationList.Models
{
    public class DataModel
    {
        DBHelper _db;
        public DataModel(string connnection)
        {
            _db = new DBHelper(connnection);
        }

        /// <summary>
        /// 查询多选框
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, dynamic>> GetParameters(int typeID)
        {
            var sql = @"SELECT  p.ID 编号,
		p.name 名称,
		t.name	类型名称
FROM    dbo.T_PE_Parameters p JOIN dbo.T_PE_ParameterTypes t ON p.TypeID=t.ID
WHERE   TypeID = @TypeID";
            var par = new SqlParameter() { ParameterName = "@TypeID", SqlDbType = System.Data.SqlDbType.Int, Value = typeID };
            return _db.GetList(sql, par);
        }
        /// <summary>
        /// 查询全部多选框
        /// </summary>
        /// <returns></returns>
        public List<Dictionary<string, dynamic>> GetParameters()
        {
            var sql = @"SELECT  p.ID 编号,
		p.name 名称,
		t.name	类型名称
FROM    dbo.T_PE_Parameters p JOIN dbo.T_PE_ParameterTypes t ON p.TypeID=t.ID";
            return _db.GetList(sql);
        }
    }
}
