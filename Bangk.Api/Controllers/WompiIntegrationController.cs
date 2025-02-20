using Bangk.Domain.Contract.Integrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class WompiIntegrationController : BaseController
    {
        private readonly ICreateBancolombiaButtonPaymentService _createBancolombiaButtonPaymentService;
        private readonly IGetTrxBancolombiaStatusService _getTrxBancolombiaStatusService;
        public WompiIntegrationController(ICreateBancolombiaButtonPaymentService createBancolombiaButtonPaymentService, IGetTrxBancolombiaStatusService getTrxBancolombiaStatusService)
        {
            _createBancolombiaButtonPaymentService = createBancolombiaButtonPaymentService;
            _getTrxBancolombiaStatusService = getTrxBancolombiaStatusService;
        }

        [HttpGet("CreateBancolombiaButtonTransfer")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateBancolombiaButtonTransfer()
        {
            var result = await _createBancolombiaButtonPaymentService.Execute();
            return Ok(result);
        }


        [HttpGet("GetTrxBancolombiaStatus")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTrxBancolombiaStatus(string Id)
        {
            var result = await _getTrxBancolombiaStatusService.Execute(Id);
            return Ok(result);
        }

    }
}
