using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminus.Server
{
    public class BaseResponse
    {
        public ResponseType Type { get; set; }
        public JObject Data { get; set; }
    }
}
