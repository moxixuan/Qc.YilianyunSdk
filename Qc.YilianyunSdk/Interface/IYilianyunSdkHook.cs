using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Qc.YilianyunSdk
{
    public interface IYilianyunSdkHook
    {
        /// <summary>
        /// 获取应用配置
        /// </summary>
        /// <returns></returns>
        YilianyunConfig GetClientConfig();
        /// <summary>
        /// 获取打印机的 AccessToken
        /// </summary>
        /// <returns></returns>
        AccessTokenOutputModel GetAccessToken(string machine_code);
        /// <summary>
        /// 保存TOKEN
        /// </summary>
        /// <returns></returns>
        YilianyunBaseOutputModel<AccessTokenOutputModel> SaveToken(AccessTokenOutputModel input);
        ///// <summary>
        ///// 移除token
        ///// </summary>
        ///// <param name="machine_code"></param>
        ///// <returns></returns>
        //Task RemoveAccessToken(string machine_code);
    }
}
