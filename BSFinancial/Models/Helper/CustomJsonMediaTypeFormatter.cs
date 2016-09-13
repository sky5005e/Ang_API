using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BSFinancial.Models.Helper
{
    public static class CustomJsonSettings
    {
        private static JsonSerializerSettings _settings;

        public static JsonSerializerSettings Instance
        {
            get
            {
                if (_settings == null)
                {
                    var settings = new JsonSerializerSettings();

                    // Must convert times coming from the client (always in UTC) to local - need both these parts:
                    settings.Converters.Add(new IsoDateTimeConverter { DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal }); // Critical part 1
                    settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;   // Critical part 2

                    // Skip circular references
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                    // Handle special cases in json (self-referencing loops, etc)
                    settings.ContractResolver = new CustomJsonResolver();

                    _settings = settings;
                }

                return _settings;
            }
        }
    }

    public class CustomJsonMediaTypeFormatter : MediaTypeFormatter
    {
        public JsonSerializerSettings _jsonSerializerSettings;

        public CustomJsonMediaTypeFormatter()
        {
            _jsonSerializerSettings = CustomJsonSettings.Instance;

            // Fill out the mediatype and encoding we support
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
            SupportedEncodings.Add(new UTF8Encoding(false, true));
        }

        public override bool CanReadType(Type type)
        {
            return true;
        }

        public override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task<Object> ReadFromStreamAsync(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
        {
            // Create a serializer
            JsonSerializer serializer = JsonSerializer.Create(_jsonSerializerSettings);

            // Create task reading the content
            return Task.Factory.StartNew(() =>
            {
                using (StreamReader streamReader = new StreamReader(stream, SupportedEncodings.First()))
                {
                    using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                    {
                        return serializer.Deserialize(jsonTextReader, type);
                    }
                }
            });
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            // Create a serializer
            JsonSerializer serializer = JsonSerializer.Create(_jsonSerializerSettings);

            // Create task writing the serialized content
            return Task.Factory.StartNew(() =>
            {
                using (StreamWriter streamWriter = new StreamWriter(stream, SupportedEncodings.First()))
                {
                    using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter))
                    {
                        serializer.Serialize(jsonTextWriter, value);
                    }
                }
            });
        }
    }

    public class CustomJsonResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            var list = base.CreateProperties(type, memberSerialization);

            // Custom stuff for my app
//             if (type == typeof(Foo))
//             {
//                 RemoveProperty(list, "Bar");
//                 RemoveProperty(list, "Bar2");
//             }

            return list;
        }

        private void RemoveProperty(IList<JsonProperty> list, string propertyName)
        {
            var rmc = list.FirstOrDefault(x => x.PropertyName == propertyName);

            if (rmc != null)
            {
                list.Remove(rmc);
            }
        }
    }
}