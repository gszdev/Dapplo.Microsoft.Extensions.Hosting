using System;
using System.IO;

namespace Dapplo.Hosting.Sample.Common
{
    public static class HostingUtility
    {
        public static string GetEnvironmentNameFromHostSettingsFile(string hostSettingsFile)
        {
            string environmentName = null;

            //var hostSettingsFilePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), hostSettingsFile);
            var hostSettingsFilePath = System.IO.Path.Combine(AppContext.BaseDirectory, hostSettingsFile);

            if (System.IO.File.Exists(hostSettingsFilePath))
            {
                var jsonData = File.ReadAllText(hostSettingsFilePath);
                var jsonHostSettingsObject = System.Text.Json.Nodes.JsonObject.Parse(jsonData);

                if (jsonHostSettingsObject != null)
                {
                    var environmentNode = jsonHostSettingsObject["environment"];
                    if (environmentNode != null && environmentNode.GetValueKind() == System.Text.Json.JsonValueKind.String)
                    {
                        environmentName = environmentNode.GetValue<string>();
                    }
                }
            }

            return environmentName;
        }
    }
}
