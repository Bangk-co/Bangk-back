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
        private readonly IGetMerchantsTokenService _getMerchantsTokenService;
        private readonly IPaymentSourcesBancolombiaTransferService _paymentSourcesBancolombiaTransferService;
        public WompiIntegrationController(ICreateBancolombiaButtonPaymentService createBancolombiaButtonPaymentService,
                                         IGetTrxBancolombiaStatusService getTrxBancolombiaStatusService,
                                         IGetMerchantsTokenService getMerchantsTokenService, IPaymentSourcesBancolombiaTransferService paymentSourcesBancolombiaTransferService )
        {
            _createBancolombiaButtonPaymentService = createBancolombiaButtonPaymentService;
            _getTrxBancolombiaStatusService = getTrxBancolombiaStatusService;
            _getMerchantsTokenService = getMerchantsTokenService;
            _paymentSourcesBancolombiaTransferService = paymentSourcesBancolombiaTransferService;
        }

        [HttpPost("CreateBancolombiaButtonTransfer")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateBancolombiaButtonTransfer()
        {
            var result = await _createBancolombiaButtonPaymentService.Execute();
            return Ok(result);
        }


        [HttpGet("GetMerchantsInfo")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMerchantsInfo()
        {
            var result = await _getMerchantsTokenService.Execute();
            return Ok(result);
        }

        [HttpPost("PaymentSourcesBancolombiaTransfer")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentSourcesBancolombiaTransfer()
        {
            var result = await _paymentSourcesBancolombiaTransferService.Execute();
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
