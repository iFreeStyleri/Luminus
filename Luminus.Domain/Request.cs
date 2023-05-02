using Luminus.Domain.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Luminus.Domain
{
    public class Request
    {
        public RequestType Type { get; set; }
        public JObject Data { get; set; }
    }
}
