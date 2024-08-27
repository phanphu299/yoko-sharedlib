using System.Net.Http;
using System.Text;
using AHI.Infrastructure.SharedKernel.Extension;

namespace AHI.Infrastructure.Audit.Extension
{
    internal static class PayloadExtension
    {
        // public static string JsonSerialize(this object value, Encoding encoding = null)
        // {
        //     encoding ??= Encoding.UTF8;
        //     using (var stream = new MemoryStream())
        //     {
        //         JsonExtension.Serialize(value, stream);
        //         return encoding.GetString(stream.ToArray());
        //     }
        // }

        public static StringContent ToJsonStringContent(this object content, Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return new StringContent(content.ToJson(), encoding, mediaType: "application/json");
        }
    }
}