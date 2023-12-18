using CloudinaryDotNet;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.VNPay;
using Donace_BE_Project.Services;
using Donace_BE_Project.Shared;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Donace_BE_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private const string SecretKey = "QKMMEYNWRZBIBWVCCTFDPOYRBSJMFELB";
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("connect")]
        public async Task<IActionResult> CreatePayment(ConnectVnPayModel input)
        {
            
            return Ok(await _paymentService.ConnectPaymentVnPayAsync(input));
        }

        [HttpGet("get-connect")]
        public async Task<IActionResult> GetConnectAsync()
        {
            return Ok(await _paymentService.GetConnectAsync());
        }
        [HttpGet("get-connect/{id}")]
        public async Task<IActionResult> GetConnectAsync(Guid id)
        {
            return Ok(await _paymentService.GetConnectAsync(id));
        }
    }
}
