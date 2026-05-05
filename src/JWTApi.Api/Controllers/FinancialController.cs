using JWTApi.Api.Response;
using JWTApi.Api.ViewModels;
using JWTApi.Api.ViewModels.Project;
using JWTApi.Application.DTOs.Financiales;
using JWTApi.Application.Services;
using JWTApi.Application.Services.Banks;
using JWTApi.Application.Services.Financiales;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinancialController : ControllerBase
    {
        private readonly FinancialService _financialService;
        public FinancialController(FinancialService financialService)
        {
            _financialService=financialService;
        }


        [HttpPost("GetFinancialDtos")]
        public async Task<IActionResult> GetFinancialDtos([FromBody] PageSizeViewModel pageSize, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _financialService.GetFinancialDtos(pageSize.PageNumber, pageSize.PageSize, cancellationToken);
            var response = new
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
            };
            return ResponseApi.Ok(response).ToHttpResponse();
        }

        [HttpPost("InsertFinancial")]
        public async Task<IActionResult> InsertFinancial([FromBody] FinancialCrudDtos financialCrudDtos, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _financialService.Insert(financialCrudDtos, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }
        [HttpPost("DeleteFinancial")]
        public async Task<IActionResult> DeleteFinancial([FromBody] ProjectDeleteViewModel projectAddViewModel, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _financialService.Delete(projectAddViewModel.Id, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }
        [HttpPost("UpdateFinancial")]
        public async Task<IActionResult> UpdateFinancial([FromBody] FinancialCrudDtos financialCrudDtos, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _financialService.Update(financialCrudDtos, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

    }
}
