using System.Text;
using System.Text.Json;

namespace Hydra.Basket.Infrastructure.Helpers
{
    public static class RedisHelper
    {
        public static byte[] SerializeToByte<T>(T entity)
        {
            var serializedObject = JsonSerializer.Serialize<T>(entity, GetOptions());

            var data  = Encoding.UTF8.GetBytes(serializedObject);

            return data;
        }

        public static T DeserializeToObject<T>(byte[] bytes)
        {
            if(bytes == null) return default(T);
            
            var bytesAsString = Encoding.UTF8.GetString(bytes);
            var obj = JsonSerializer.Deserialize<T>(bytesAsString, GetOptions());

            return obj;
        }

        private static JsonSerializerOptions GetOptions() =>
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
    }
}