using System.Threading.Tasks;
using System.IO;
using System;
using Newtonsoft.Json;

namespace GoogleCalendarCopier.Configuration {
    public class JsonConfigurationProvider : IConfigurationProvider
    {
        private Configuration _config;
        private string _filepath;

        public JsonConfigurationProvider(string filepath) {
            _filepath = filepath;
            if (!File.Exists(_filepath)) {
                File.WriteAllText(@_filepath, JsonConvert.SerializeObject(new Configuration()));
            }
        }

        public Task<Configuration> Read() {
            if (this._config == null)
            {
                try {
                    this._config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(this._filepath));
                } catch (Exception ex) {
                    Console.WriteLine("Error reading configuration.", ex.ToString());
                    this._config = new Configuration();
                }
            }
            
            return Task.FromResult(this._config);
        }
    }
}