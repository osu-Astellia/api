using ServiceStack.Redis;

namespace AstelliaAPI
{
    public static class Global
    {
        public static readonly RedisClient Redis = new RedisClient();
    }
}