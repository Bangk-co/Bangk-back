using Bangk.Common.Dto;
using Bangk.Domain.Contract.Integrations;
using Bangk.Domain.Contract.Integrations.Wompi;
using Bangk.Domain.Implementation.Integrations.Wompi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class WompiIntegrationController : BaseController
    {
        private readonly ICreateBancolombiaButtonPaymentService _createBancolombiaButtonPaymentService;
        private readonly IGetTrxBancolombiaStatusService _getTrxBancolombiaStatusService;
        private readonly IGetMerchantsTokenService _getMerchantsTokenService;
        private readonly IPaymentSourcesBancolombiaTransferService _paymentSourcesBancolombiaTransferService;
        private readonly ICardPaymentService _cardPaymentService;
        private readonly IGenerateIntegrityKeyService _generateIntegrityKeyService;

        public WompiIntegrationController(ICreateBancolombiaButtonPaymentService createBancolombiaButtonPaymentService,
                                         IGetTrxBancolombiaStatusService getTrxBancolombiaStatusService,
                                         IGetMerchantsTokenService getMerchantsTokenService, IPaymentSourcesBancolombiaTransferService paymentSourcesBancolombiaTransferService,
                                         ICardPaymentService cardPaymentService, IGenerateIntegrityKeyService generateIntegrityKeyService)
        {
            _createBancolombiaButtonPaymentService = createBancolombiaButtonPaymentService;
            _getTrxBancolombiaStatusService = getTrxBancolombiaStatusService;
            _getMerchantsTokenService = getMerchantsTokenService;
            _paymentSourcesBancolombiaTransferService = paymentSourcesBancolombiaTransferService;
            _cardPaymentService = cardPaymentService;
            _generateIntegrityKeyService = generateIntegrityKeyService;
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
        public async Task<IActionResult> PaymentSourcesBancolombiaTransfer(string transactionNumber, int value)
        {
            var result = await _paymentSourcesBancolombiaTransferService.Execute(transactionNumber, value);
            return Ok(result);
        }


        [HttpGet("GetTrxBancolombiaStatus")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTrxBancolombiaStatus(string Id)
        {
            var result = await _getTrxBancolombiaStatusService.Execute(Id);
            return Ok(result);
        }

        [HttpPost("CardPayment")]
        [AllowAnonymous]
        public async Task<IActionResult> CardPayment(CardPaymentDto cardPaymentDto)
        {
            var result = await _cardPaymentService.Execute(cardPaymentDto);
            return Ok(result);
        }


        [HttpPost("GenerateSignature")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateSignature(int value)
        {
            var result = await _generateIntegrityKeyService.Execute(value, Guid.NewGuid().ToString("N"));
            return Ok(result);
        }
    }
}
