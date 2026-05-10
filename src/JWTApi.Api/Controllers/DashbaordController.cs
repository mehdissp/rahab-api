using JWTApi.Api.Response;
using JWTApi.Application.Services.Companies;
using JWTApi.Application.Services.Financiales;
using JWTApi.Application.Services.FinancialOperationses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashbaordController : ControllerBase
    {
        private readonly FinancialOperationsService _financialService;
        public DashbaordController(FinancialOperationsService financialService)
        {
            _financialService = financialService;
        }
        [HttpGet("GetDashboardsDtosAsync")]
        public async Task<IActionResult> GetDashboardsDtosAsync(CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _financialService.GetDashboardsDtosAsync(1, userId,cancellationToken);

            return ResponseApi.Ok(result).ToHttpResponse();
        }
    }
}
