using GreenhouseController.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.APICallers
{
    public class APICaller
    {
        // Base URL for API
        private const string _baseUrl = "http://sample-env.549mj2hqpd.us-west-2.elasticbeanstalk.com";

        // API Key for HTTP POSTs
        private const string _apiKey = "44ffe28b-f470-4bc0-8ee9-38fce01438ce";

        // Empty constructor
        public APICaller() { }

        /// <summary>
        /// Performs an HTTP GET to the greenhouse API and returns the resulting JSON
        /// </summary>
        /// <param name="path">API path to send GET to, such as "/api/sensors"</param>
        /// <returns></returns>
        public string GetGreenhouseData(string path)
        {
            string result = "";
            try
            {
                // Create our http request using the path specified, and set the type as JSOn
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(_baseUrl + path);
                req.ContentType = "application/json; charset=utf-8";

                // Get the response from our GET
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

                using (Stream responseStream = resp.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
                    
            return result;
        }

        /// <summary>
        /// Sends an HTTP POST t
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool PostGreenhouseState(string path)
        {
            // Get the current state of the greenhouse and put it into a form the API can handle
            HardwareStatePacket greenhouseState = new HardwareStatePacket();
            greenhouseState.apikey = _apiKey;
            #region GetGreenhouseStates
            if (StateMachineContainer.Instance.Temperature.CurrentState == GreenhouseState.COOLING)
            {
                greenhouseState.fans = true.ToString();
                greenhouseState.heater = false.ToString();
                greenhouseState.vents = true.ToString();
            }
            else if (StateMachineContainer.Instance.Temperature.CurrentState == GreenhouseState.HEATING)
            {
                greenhouseState.fans = true.ToString();
                greenhouseState.heater = true.ToString();
                greenhouseState.vents = false.ToString();
            }
            else
            {
                greenhouseState.fans = false.ToString();
                greenhouseState.heater = false.ToString();
                greenhouseState.vents = false.ToString();
            }
            if (StateMachineContainer.Instance.Shading.CurrentState == GreenhouseState.SHADING)
            {
                greenhouseState.shades = true.ToString();
            }
            else
            {
                greenhouseState.shades = false.ToString();
            }
            if(StateMachineContainer.Instance.WateringStateMachines.Where(s => s.CurrentState == GreenhouseState.WATERING).Count() != 0)
            {
                greenhouseState.pump = true.ToString();
            }
            else
            {
                greenhouseState.pump = false.ToString();
            }
            if(StateMachineContainer.Instance.LightStateMachines.Where(s => s.CurrentState == GreenhouseState.LIGHTING).Count() != 0)
            {
                greenhouseState.lights = true.ToString();
            }
            else
            {
                greenhouseState.lights = false.ToString();
            }
            #endregion
            string greenhouseStateText = $"heater={greenhouseState.heater}&apikey={greenhouseState.apikey}"+
                $"&shades={greenhouseState.shades}&fans={greenhouseState.fans}&lights={greenhouseState.lights}&pump={greenhouseState.pump}&vents={greenhouseState.vents}";

            try
            {
                // Create the request and set the correct properties
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(_baseUrl + path);
                //req.ContentType = "application/json; charset=utf-8";
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";

                // Write the JSON to the stream
                using (StreamWriter writer = new StreamWriter(req.GetRequestStream()))
                {
                    //Console.WriteLine(greenhouseStateJson);
                    writer.Write(greenhouseStateText);
                    writer.Flush();
                }

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                using (var streamReader = new StreamReader(resp.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
