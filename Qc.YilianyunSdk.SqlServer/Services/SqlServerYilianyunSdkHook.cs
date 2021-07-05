using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Qc.YilianyunSdk.SqlServer.Infrastructure;
using Qc.YilianyunSdk.SqlServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qc.YilianyunSdk.SqlServer.Services
{
    public class SqlServerYilianyunSdkHook : IYilianyunSdkHook
    {
        private readonly YilianyunSqlServerConfig _yilianyunConfig;
        private readonly IMemoryCache _cache;

        public SqlServerYilianyunSdkHook(IOptions<YilianyunSqlServerConfig> configOptions, IMemoryCache cache)
        {
            _yilianyunConfig = configOptions?.Value;
            _cache = cache;
        }

        /// <summary>
        /// 获取应用配置
        /// </summary>
        /// <returns></returns>
        public YilianyunConfig GetClientConfig()
        {
            return _yilianyunConfig;
        }

        public AccessTokenOutputModel GetAccessToken(string machine_code)
        {
            return _yilianyunConfig.YilianyunClientType switch
            {
                YilianyunClientType.开放应用 => throw new NotImplementedException(),
                YilianyunClientType.自有应用 => _cache.GetOrCreate($"{GetType().FullName}_{_yilianyunConfig.ClientId}", entry =>
                {
                    entry.SlidingExpiration = _yilianyunConfig.TokenSlidingExpiration;
                    using var context = new YilianyunContext(_yilianyunConfig.SaveConnection);
                    return context.AccessTokenOutputModels.AsNoTracking().FirstOrDefault(f => f.Machine_Code == _yilianyunConfig.ClientId);
                }),
                _ => throw new NotImplementedException(),
            };
        }

        public YilianyunBaseOutputModel<AccessTokenOutputModel> SaveToken(AccessTokenOutputModel input)
        {
            _ = _yilianyunConfig.YilianyunClientType switch
            {
                YilianyunClientType.开放应用 => throw new NotImplementedException(),
                YilianyunClientType.自有应用 => input.Machine_Code = _yilianyunConfig.ClientId,
                _ => throw new NotImplementedException(),
            };

            using var context = new YilianyunContext(_yilianyunConfig.SaveConnection);
            if(context.AccessTokenOutputModels.AsNoTracking().Any(a => a.Machine_Code == input.Machine_Code))
            {
                context.Update(input);
            }
            else
            {
                context.Add(input);
            }
            context.SaveChanges();
            return new YilianyunBaseOutputModel<AccessTokenOutputModel>("授权成功", "0") { Body = input };
        }

        //public async Task RemoveAccessToken(string machine_code)
        //{
        //    _cache.Remove($"{GetType().FullName}_{machine_code}");
        //    using var context = new YilianyunContext(_yilianyunConfig.SaveConnection);
        //    var accessTokenOutputModel = await context.AccessTokenOutputModels.FirstOrDefaultAsync(f => f.Machine_Code == machine_code);
        //    if (accessTokenOutputModel != null)
        //    {
        //        context.AccessTokenOutputModels.Remove(accessTokenOutputModel);
        //    }
        //    await context.SaveChangesAsync();
        //}
    }
}
