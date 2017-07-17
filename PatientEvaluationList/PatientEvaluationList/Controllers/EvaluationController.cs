using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using PatientEvaluationList.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatientEvaluationList.Controllers
{
    public class EvaluationController: Controller
    {
        DataModel _db;
        /// <summary>
        /// 日志对象
        /// </summary>
        Logger _log;
        /// <summary>
        /// 配置文件
        /// </summary>
        /// <summary>
        /// 构函初始化
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="appseting"></param>
        public EvaluationController(IOptions<ConnectionSetting> connections)
        {
            _log = LogManager.GetCurrentClassLogger();
            _db = new DataModel(connections.Value.hisConnectionStrings);
        }
        [HttpGet("EvaluationIndex")]
        public IActionResult EvaluationIndex()
        {
            return View();
        }

        /// <summary>
        /// 获取复选框
        /// </summary>
        /// <param name="typeid"></param>
        /// <returns></returns>
        [HttpGet("getparameters/typeid")]
        public IActionResult GetParameters(int typeid)
        {
            try
            {
                var list = _db.GetParameters(typeid);
                return new JsonResult(new { result = 1, message = "查询成功", data = list },new JsonSerializerSettings()
                {
                    DateFormatString = "yyyy-MM-dd"
                    //,ContractResolver = new LowercaseContractResolver()
                });
            }
            catch (Exception exc)
            {

                _log.Fatal($"【获取报告列表异常】：{exc.Message}，typeid：{typeid}");
                return new JsonResult(new { result = 0, messge = exc.Message });
            }
           
        }
        /// <summary>
        /// 获取全部复选框
        /// </summary>
        /// <returns></returns>
        [HttpGet("getparameters")]
        public IActionResult GetParameters()
        {
            try
            {
                var list = _db.GetParameters();
                return new JsonResult(new { result = 1, message = "查询成功", data = list }, new JsonSerializerSettings()
                {
                    DateFormatString = "yyyy-MM-dd"
                    //,ContractResolver = new LowercaseContractResolver()
                });
            }
            catch (Exception exc)
            {

                _log.Fatal($"【获取报告列表异常】：{exc.Message}");
                return new JsonResult(new { result = 0, messge = exc.Message });
            }

        }
    }
}
