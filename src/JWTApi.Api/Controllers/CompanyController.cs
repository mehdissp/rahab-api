using JWTApi.Api.Response;
using JWTApi.Api.ViewModels;
using JWTApi.Api.ViewModels.Company;
using JWTApi.Api.ViewModels.Project;
using JWTApi.Application.Services;
using JWTApi.Application.Services.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWTApi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _companyService;
        public CompanyController(CompanyService companyService)
        {
            _companyService = companyService;
        }


        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromBody] CompanyViewModel companyViewModel, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _companyService.Insert(companyViewModel.Name, companyViewModel.DescriptionRows, userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpPost("Delete")]
        public async Task<IActionResult> Delete([FromBody] int id, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _companyService.Delete(id, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] CompanyViewModel companyViewModel, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            await _companyService.Update(companyViewModel.Id,companyViewModel.Name,companyViewModel.DescriptionRows,userId, cancellationToken);

            return ResponseApi.Ok().ToHttpResponse();

        }
        [HttpPost("GetCompany")]
        public async Task<IActionResult> GetCompany([FromBody] PageSizeViewModel pageSize, CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _companyService.GetCompany(pageSize.PageNumber, pageSize.PageSize, cancellationToken);
            var response = new
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages,
            };
            return ResponseApi.Ok(response).ToHttpResponse();
        }

        [HttpGet("GetCompanyCombo")]
        public async Task<IActionResult> GetCompanyCombo(CancellationToken cancellationToken)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var result = await _companyService.CompanyDtos( cancellationToken);
         
            return ResponseApi.Ok(result).ToHttpResponse();
        }
    }
}
