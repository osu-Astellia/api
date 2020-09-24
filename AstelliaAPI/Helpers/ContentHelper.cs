using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AstelliaAPI.Helpers
{
    public class ContentHelper
    {
        public static ContentResult GenerateError(object message)
        {
            var dict = new Dictionary<string, object>();
            var result = new ContentResult();
            dict["result"] = message;
            result.ContentType = "application/json";
            result.StatusCode = 401;
            result.Content = JsonConvert.SerializeObject(dict);
            return result;
        }

        public static ContentResult NoTokenError(){
            var dict = new Dictionary<string, object>();
            var result = new ContentResult();
            dict["message"] = "Unauthorized";
            result.ContentType = "application/json";
            result.StatusCode = 401;
            result.Content = JsonConvert.SerializeObject(dict);
            return result;
        }

        public static ContentResult Message(object message, int code = 200){
            var dict = new Dictionary<string, object>();
            var result = new ContentResult();
            dict["message"] = message;
            result.ContentType = "application/json";
            result.StatusCode = code;
            result.Content = JsonConvert.SerializeObject(dict);
            return result;
        }


        public static ContentResult GenerateErrorCustom<T>(T message)
        {
            var result = new ContentResult();
            result.ContentType = "application/json";
            result.StatusCode = 401;
            result.Content = JsonConvert.SerializeObject(message);
            return result;
        }
        public static ContentResult GenerateOk(object message)
        {
            var dict = new Dictionary<string, object>();
            var result = new ContentResult();
            dict["result"] = message;
            result.ContentType = "application/json";
            result.StatusCode = 200;
            result.Content = JsonConvert.SerializeObject(dict);
            return result;
        }
        public async static Task<ContentResult> GenerateOkAsync(object message)
        {
            var dict = new Dictionary<string, object>();
            var result = new ContentResult();
            dict["result"] = message;
            result.ContentType = "application/json";
            result.StatusCode = 200;
            result.Content = JsonConvert.SerializeObject(dict);
            return result;
        }
        public static ContentResult GenerateOkCustom<T>(T message)
        {
            var result = new ContentResult();
            result.ContentType = "application/json";
            result.StatusCode = 200;
            result.Content = JsonConvert.SerializeObject(message);
            return result;
        }
    }
}
