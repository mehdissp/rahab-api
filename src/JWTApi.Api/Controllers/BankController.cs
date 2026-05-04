using JWTApi.Api.Response;
using JWTApi.Api.ViewModels;
using JWTApi.Api.ViewModels.Bank;
using JWTApi.Api.ViewModels.Company;
using JWTApi.Application.Services.Banks;
using JWTApi.Application.Services.Companies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly BankService _bankService;
        public BankController(BankService bankService)
        {
            _bankService = bankService;
        }
        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] BankViewModel bankViewModel, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _bankService.Insert(bankViewModel.Name,bankViewModel.Address,bankViewModel.Phone, bankViewModel.Desc, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }


        [HttpPost("GetBank")]
        public async Task<IActionResult> GetBankDtosAsync([FromBody] PageSizeViewModel pageSize, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _bankService.GetBankDtosAsync(pageSize.PageNumber, pageSize.PageSize, cancellationToken);
            var response = new
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
            };
            return ResponseApi.Ok(response).ToHttpResponse();
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromBody] int id, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _bankService.Delete(id, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] BankViewModel bankViewModel, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _bankService.Update(bankViewModel.Id, bankViewModel.Name,bankViewModel.Address,bankViewModel.Phone
                , bankViewModel.Desc, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }


    }
}
