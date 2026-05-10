using JWTApi.Api.Response;
using JWTApi.Api.ViewModels;
using JWTApi.Api.ViewModels.FinancialOperations;
using JWTApi.Application.DTOs.Financiales;
using JWTApi.Application.DTOs.FinancialOperationses;
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
    public class FinancialOperationsController : ControllerBase
    {
        private readonly FinancialOperationsService _financialService;
        public FinancialOperationsController(FinancialOperationsService financialService)
        {
            _financialService = financialService;
        }
        [HttpPost("FinancialOperationsDtos")]
        public async Task<IActionResult> FinancialOperationsDtos([FromBody] PageSizeOPViewModel pageSize, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _financialService.GetFinancialOperations(pageSize.ProjectId, pageSize.BankId, pageSize.KeyValue, pageSize.PageNumber, pageSize.PageSize, cancellationToken);
            var response = new
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
            };
            return ResponseApi.Ok(response).ToHttpResponse();
        }

        [HttpPost("InsertFinancialOperations")]
        public async Task<IActionResult> InsertFinancialOperations([FromBody] FinancialOperationsCrudDtos financialCrudDtos, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _financialService.Insert(financialCrudDtos, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }
        [HttpPost("UpdateFinancialOperations")]
        public async Task<IActionResult> UpdateFinancialOperations([FromBody] FinancialOperationsCrudDtos financialCrudDtos, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _financialService.Update(financialCrudDtos, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }
        [HttpPost("DeleteFinancialOperations")]
        public async Task<IActionResult> DeleteFinancialOperations([FromBody] int id, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _financialService.Delete(id, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpPost("GetComboCompany")]
        public async Task<IActionResult> GetComboCompany( CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
          var check=  await _financialService.GetComboCompany( userId, cancellationToken);

            return ResponseApi.Ok(check).ToHttpResponse();

        }
        [HttpPost("GetComboBank")]
        public async Task<IActionResult> GetComboBank([FromBody] int id, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var check = await _financialService.GetComboBank(userId, id, cancellationToken);

            return ResponseApi.Ok(check).ToHttpResponse();

        }
        [HttpPost("GetComboProject")]
        public async Task<IActionResult> GetComboProject([FromBody] int id, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var check = await _financialService.GetComboProject(userId, id, cancellationToken);

            return ResponseApi.Ok(check).ToHttpResponse();

        }
        [HttpPost("GetComboFinancialsAsync")]
        public async Task<IActionResult> GetComboFinancialsAsync([FromBody] FinancialOperationsViewModel financialOperationsViewModel, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var check = await _financialService.GetComboFinancialsAsync(userId,
                financialOperationsViewModel.Id, financialOperationsViewModel.financialTransactionType,
                cancellationToken);

            return ResponseApi.Ok(check).ToHttpResponse();

        }

        [HttpPost("GetAccountSideCombo")]
        public async Task<IActionResult> GetAccountSideCombo([FromBody] int id , CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var check = await _financialService.GetAccountSideCombo(userId,
               id,
                cancellationToken);

            return ResponseApi.Ok(check).ToHttpResponse();

        }



    }
}
