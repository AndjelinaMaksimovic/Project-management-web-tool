using System;
using System.IO;
using Newtonsoft.Json;

namespace Codedberries.Helpers
{
    public class ErrorMsg
    {
        public string ErrorMessage { get; set; }

        public ErrorMsg(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
