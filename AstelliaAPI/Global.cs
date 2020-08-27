using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstelliaAPI
{
    public static class Global
    {
        public static RedisClient Redis = new RedisClient();
    }
}
