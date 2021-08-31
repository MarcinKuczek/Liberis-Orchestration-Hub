using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Liberis.OrchestrationHub.Core.Converters
{
    public static class GuidConverter
    {
        public static Guid ConvertObject(object input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(JsonConvert.SerializeObject(input)));
                return new Guid(hash);
            }
        }
    }
}
