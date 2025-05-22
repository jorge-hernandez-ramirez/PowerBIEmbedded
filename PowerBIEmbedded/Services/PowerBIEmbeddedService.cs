using Microsoft.Identity.Client;
using Newtonsoft.Json; 
using Newtonsoft.Json.Linq;
using PowerBIEmbedded.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PowerBIEmbedded.Services
{
    public class PowerBIEmbeddedService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PowerBIEmbeddedService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // Método para obtener el token de acceso mediante Client Credentials
        private async Task<string> GetAccessTokenAsync()
        {
            var tenantId = _configuration["PowerBIEmbedded:TenantId"];
            var clientId = _configuration["PowerBIEmbedded:ClientId"];
            var clientSecret = _configuration["PowerBIEmbedded:ClientSecret"];

            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
                .Build();

            string[] scopes = new string[] { "https://analysis.windows.net/powerbi/api/.default" };

            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.AccessToken;
        }

        // Método para llamar al endpoint de Power BI que genera el Embed Token y conforma la configuración
        public async Task<ReportEmbeddedConfiguration> GetReportEmbedConfigAsync(string reportId)
        {
            // Obtener access token para la API de Power BI
            var accessToken = await GetAccessTokenAsync();

            // Configurar el HttpClient con el token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            // Recuperar el WorkspaceId (Grupo) y componer la URL del API
            var workspaceId = _configuration["PowerBIEmbedded:WorkspaceId"];
            var EmbedUrl = _configuration["PowerBIEmbedded:EmbedUrl"];
            var requestUrl = $"https://api.powerbi.com/v1.0/myorg/groups/{workspaceId}/reports/{EmbedUrl}/GenerateToken";

            // Payload: se solicita el acceso en modo 'view'
            var payload = new { accessLevel = "view" };

            // Llamar al endpoint de generación del token
            var response = await _httpClient.PostAsJsonAsync(requestUrl, payload);
            response.EnsureSuccessStatusCode();

            // Imprimir el contenido de la respuesta
            // deserializar la respuesta
            // var tokenResponse = JsonSerializer.Deserialize<EmbedTokenResponse>(await response.Content.ReadAsStringAsync());
            var tokenResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
            if (tokenResponse == null)
                throw new Exception("Error al obtener el token de Power BI.");

             Console.WriteLine(tokenResponse);
             if (tokenResponse == null)
             {
                 throw new Exception("Error al obtener el token de Power BI.");
             }

            // Construimos el objeto de configuración que enviaremos al frontend.
            var reportEmbeddedConfiguration = new ReportEmbeddedConfiguration
            {
                ReportId = reportId,
                //
                // Puedes obtener el embed URL desde la respuesta de otra API o configurarlo previamente:
                EmbeddedUrl = "https://app.powerbi.com/reportEmbed?reportId="+reportId, 
                Token = tokenResponse["token"].ToString(),
                TokenExpiration = tokenResponse["expiration"].ToString(),

            };

            return reportEmbeddedConfiguration;
        }
    }
}
