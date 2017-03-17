using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace com.yannis.utils
{
    /// <summary>
    /// 用户自定义配置读取类
    /// </summary>
    public static class CustomerConfigure
    {

        /// <summary>
        /// 验证码生成所需字体资源文件路径
        /// </summary>
        public static string VerifyCodeResourcePath
        {
            get
            {
                return ConfigurationManager.AppSettings["verifyCodeResourcePath"] ?? string.Empty;
            }
        }


    }
}
