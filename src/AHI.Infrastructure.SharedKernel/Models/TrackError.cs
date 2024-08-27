using System.Collections.Generic;
using AHI.Infrastructure.SharedKernel.Constant;
using Newtonsoft.Json;

namespace AHI.Infrastructure.SharedKernel.Model
{
    public class TrackError
    {
        [JsonProperty(Order = -2)]
        public string Type { get; set; }

        public string Message { get; set; }

        public IDictionary<string, object> ValidationInfo { get; set; }

        public TrackError(
            string message,
            ErrorType errorType = ErrorType.UNDEFINED,
            IDictionary<string, object> validationInfo = null
        )
        {
            this.Message = message;
            this.Type = errorType.ToString();
            ValidationInfo = validationInfo;
        }
    }
}
