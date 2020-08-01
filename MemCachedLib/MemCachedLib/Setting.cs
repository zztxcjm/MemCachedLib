using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemCachedLib
{
    public class Setting
    {
        /// <summary>
        /// 连接池最大连接数量
        /// </summary>
        public static int PoolMaxSize = 10;

        /// <summary>
        /// 最大尝试连接次数
        /// </summary>
        public static int TryConnectMaxCount = 30;

        /// <summary>
        /// 数据最大尺寸限制（单位:byte,默认1MB）
        /// </summary>
        public static long DataMaxSize = 1 * 1024 * 1024;

    }
}
