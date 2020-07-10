using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AstelliaAPI.Helpers
{
    public class ContentHelper
    {
        public static ContentResult GenerateError(object message)
        {
            var dict = new Dictionary<string, object> { ["result"] = message };
            var result = new ContentResult
            {
                ContentType = "application/json", StatusCode = 401, Content = JsonConvert.SerializeObject(dict)
            };

            return result;
        }

        public static ContentResult GenerateErrorCustom<T>(T message)
        {
            var result = new ContentResult
            {
                ContentType = "application/json", StatusCode = 401, Content = JsonConvert.SerializeObject(message)
            };
            return result;
        }

        public static ContentResult GenerateOk(object message)
        {
            var dict = new Dictionary<string, object> { ["result"] = message };
            var result = new ContentResult
            {
                ContentType = "application/json", StatusCode = 200, Content = JsonConvert.SerializeObject(dict)
            };

            return result;
        }

        public static ContentResult GenerateOkCustom<T>(T message)
        {
            var result = new ContentResult
            {
                ContentType = "application/json", StatusCode = 200, Content = JsonConvert.SerializeObject(message)
            };
            return result;
        }
    }
}