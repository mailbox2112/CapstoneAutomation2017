using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.APICallers
{
    public class APICaller
    {
        private const string _baseUrl = "http://sample-env.549mj2hqpd.us-west-2.elasticbeanstalk.com/api/";

        public APICaller() { }

        public JObject Get(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            return null;
        }
    }
}
