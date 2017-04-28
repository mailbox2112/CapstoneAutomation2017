using GreenhouseController.APICallers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.API
{
    public class APIDataHandler
    {
        private List<string> _apiPaths = new List<string>() { "/api/sensors", "/api/automation", "/api/manualcontrols", "/api/state" };
        private APICaller _apiCaller;

        public APIDataHandler(APICaller caller)
        {
            _apiCaller = caller;
        }

        public void RequestDataFromAPI()
        {
            try
            {
                SensorPacket[] sensorData = new SensorPacket[6];
                ManualControlPacket manualControls = new ManualControlPacket();
                AutomationPacket automationData = new AutomationPacket();
                foreach (string path in _apiPaths)
                {
                    string result = _apiCaller.GetGreenhouseData(path);
                    switch (path)
                    {
                        case "/api/sensors":
                            sensorData = JsonConvert.DeserializeObject<SensorPacket[]>(result);
                            break;
                        case "/api/manualcontrols":
                            manualControls = JsonConvert.DeserializeObject<ManualControlPacket>(result);
                            break;
                        case "/api/automation":
                            automationData = JsonConvert.DeserializeObject<AutomationPacket>(result);
                            break;
                        case "/api/state":
                            bool success = _apiCaller.PostGreenhouseState(path);
                            if (!success)
                            {
                                Console.WriteLine("Could not successfully POST greenhouse state.");
                            }
                            break;
                        default:
                            break;
                    }
                }

                DataAnalyzer analyzer = new DataAnalyzer();
                analyzer.ExecuteActions(sensorData, manualControls, automationData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
