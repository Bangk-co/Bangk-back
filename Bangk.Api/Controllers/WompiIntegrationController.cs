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
        private readonly IGenerateTokenTrxBancolombiaService _createBancolombiaButtonPaymentService;
        private readonly IGetTrxBancolombiaStatusService _getTrxBancolombiaStatusService;
        private readonly IGetMerchantsTokenService _getMerchantsTokenService;
        private readonly IPaymentSourcesBancolombiaTransferService _paymentSourcesBancolombiaTransferService;
        private readonly ICardPaymentService _cardPaymentService;
        private readonly IGenerateIntegrityKeyService _generateIntegrityKeyService;
        private readonly ITransactionUpdateStateService _transactionUpdateStateService;
        public WompiIntegrationController(IGenerateTokenTrxBancolombiaService createBancolombiaButtonPaymentService,
                                         IGetTrxBancolombiaStatusService getTrxBancolombiaStatusService,
                                         IGetMerchantsTokenService getMerchantsTokenService, IPaymentSourcesBancolombiaTransferService paymentSourcesBancolombiaTransferService,
                                         ICardPaymentService cardPaymentService, IGenerateIntegrityKeyService generateIntegrityKeyService, ITransactionUpdateStateService transactionUpdateStateService)
        {
            _createBancolombiaButtonPaymentService = createBancolombiaButtonPaymentService;
            _getTrxBancolombiaStatusService = getTrxBancolombiaStatusService;
            _getMerchantsTokenService = getMerchantsTokenService;
            _paymentSourcesBancolombiaTransferService = paymentSourcesBancolombiaTransferService;
            _cardPaymentService = cardPaymentService;
            _generateIntegrityKeyService = generateIntegrityKeyService;
            _transactionUpdateStateService = transactionUpdateStateService;
        }

        [HttpPost("GenerateTokenTrxBancolombia")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateTokenTrxBancolombia()
        {
            var result = await _createBancolombiaButtonPaymentService.Execute();
            return Ok(result);
        }

        [HttpPost("PaymentSourcesBancolombiaTransfer")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentSourcesBancolombiaTransfer(UserPaymentRequest userPaymentRequest)
        {
            var result = await _paymentSourcesBancolombiaTransferService.Execute(userPaymentRequest);
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
        public async Task<ActionResult<ResponseDto<PaymentCardResponseDto>>> CardPayment(CardPaymentDto cardPaymentDto)
        {
            var result = await _cardPaymentService.Execute(cardPaymentDto);
            return Ok(result);
        }

        [HttpPost("TransactionUpdateState")]
        [AllowAnonymous]
        public async Task<IActionResult> TransactionUpdateState([FromBody] UpdateTransactionWebHookDto transactionUpdateDto)
        {
            if (transactionUpdateDto == null)
            {
                return BadRequest("Invalid transaction data");
            }

            await _transactionUpdateStateService.Execute(transactionUpdateDto);
            return Ok();
        }
    }
}
