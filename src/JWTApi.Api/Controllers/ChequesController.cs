using JWTApi.Api.Response;
using JWTApi.Api.ViewModels;
using JWTApi.Application.Services.Cheques;
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
    public class ChequesController : ControllerBase
    {
        private readonly ChequesService _chequesService;
        public ChequesController(ChequesService chequesService)
        {
            _chequesService = chequesService;
        }
        [HttpPost("ChequesDtosAsync")]
        public async Task<IActionResult> ChequesDtosAsync([FromBody] PageSizeViewModel pageSize, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _chequesService.ChequesDtosAsync(pageSize.Id, userId,pageSize.PageNumber, pageSize.PageSize,pageSize.KeyValue, cancellationToken);
            var response = new
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
            };
            return ResponseApi.Ok(response).ToHttpResponse();
        }
    }
}
