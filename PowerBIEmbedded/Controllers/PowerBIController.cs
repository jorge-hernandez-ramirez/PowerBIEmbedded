using Microsoft.AspNetCore.Mvc;
using PowerBIEmbedded.Services;

namespace PowerBIEmbedded.Controllers
{
    [ApiController]
    [Route("PowerBI")]
    public class PowerBIController : ControllerBase
    {
        private readonly PowerBIEmbeddedService _powerBIEmbeddedService;

        
        public PowerBIController(PowerBIEmbeddedService powerBIEmbeddedService)
        {
            _powerBIEmbeddedService = powerBIEmbeddedService;
        }

        // Endpoint para obtener la configuración del reporte (embed token, embed url, etc.)
        [HttpGet("reportConfiguration")]
        public async Task<IActionResult> GetReportConfiguration([FromQuery] string reportId)
        {
            var embedConfig = await _powerBIEmbeddedService.GetReportEmbedConfigAsync(reportId);
            return Ok(embedConfig);
        }
    }
}
