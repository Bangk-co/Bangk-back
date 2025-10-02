using Bangk.Common.Dto;
using Bangk.Domain.Contract.Integrations;
using Bangk.Domain.Contract.Integrations.Wompi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Bangk.Api.Controllers
{
    public class WompiIntegrationController : BaseController
    {
        private readonly IGenerateTokenTrxBancolombiaService _createBancolombiaButtonPaymentService;
        private readonly IGetTrxBancolombiaStatusService _getTrxBancolombiaStatusService;
        private readonly IPaymentSourcesBancolombiaTransferService _paymentSourcesBancolombiaTransferService;
        private readonly ICardPaymentService _cardPaymentService;
        private readonly ITransactionUpdateStateService _transactionUpdateStateService;
        private readonly IGetStatusTransactionService _getStatusTransactionService;
        private readonly IGetBancolombiaTransactionIdService _getBancolombiaTransactionIdService;
        public WompiIntegrationController(IGenerateTokenTrxBancolombiaService createBancolombiaButtonPaymentService,
                                         IGetTrxBancolombiaStatusService getTrxBancolombiaStatusService,
                                         IGetMerchantsTokenService getMerchantsTokenService,
                                         IPaymentSourcesBancolombiaTransferService paymentSourcesBancolombiaTransferService,
                                         ICardPaymentService cardPaymentService,
                                         IGenerateIntegrityKeyService generateIntegrityKeyService, 
                                         ITransactionUpdateStateService transactionUpdateStateService,
                                         IGetStatusTransactionService getStatusTransactionService,
                                         IGetBancolombiaTransactionIdService getBancolombiaTransactionIdService)
        {
            _createBancolombiaButtonPaymentService = createBancolombiaButtonPaymentService;
            _getTrxBancolombiaStatusService = getTrxBancolombiaStatusService;
            _paymentSourcesBancolombiaTransferService = paymentSourcesBancolombiaTransferService;
            _cardPaymentService = cardPaymentService;
            _transactionUpdateStateService = transactionUpdateStateService;
            _getStatusTransactionService = getStatusTransactionService;
            _getBancolombiaTransactionIdService = getBancolombiaTransactionIdService;
        }

        [HttpPost("GenerateTokenTrxBancolombia")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateTokenTrxBancolombia(int applicationId, bool isMed)
        {
            var result = await _createBancolombiaButtonPaymentService.Execute(applicationId, isMed);
            return Ok(result);
        }

        [HttpPost("PaymentSourcesBancolombiaTransfer")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentSourcesBancolombiaTransfer(RequestUserPaymentDto userPaymentRequest)
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
            if (result.HasErrors)
            {
                return BadRequest(result);
            }
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

        [HttpGet("GetStatusTransaction")]
        [AllowAnonymous]
        public async Task<ActionResult<ResponseDto<string>>> GetStatusTransaction(string transactionReference)
        {
            var result = await _getStatusTransactionService.Execute(transactionReference);
            if (result.HasErrors)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("GetBancolombiaTransactionId")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> GetBancolombiaTransactionId(int applicationId, bool isMed)
        {
            var result = await _getBancolombiaTransactionIdService.Execute(applicationId, isMed);
            return Ok(result);
        }
    }
}
