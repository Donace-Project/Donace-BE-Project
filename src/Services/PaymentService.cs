using AutoMapper;
using CloudinaryDotNet.Actions;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Entities.Payment;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.VNPay;
using Donace_BE_Project.Shared;
using Nest;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Data;

namespace Donace_BE_Project.Services
{
    public class PaymentService : IPaymentService
    {
        public readonly ILogger<PaymentService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebManageService _webManageService;
        private readonly IConnectPaymentRepository _connectPaymentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentService(ILogger<PaymentService> logger, 
                              IConfiguration configuration,
                              IHttpContextAccessor contextAccessor,
                              IWebManageService webManageService,
                              IConnectPaymentRepository connectPaymentRepository,
                              IMapper mapper, 
                              IUnitOfWork unit)
        {
            _logger = logger;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _webManageService = webManageService;
            _connectPaymentRepository = connectPaymentRepository;
            _mapper = mapper;
            _unitOfWork = unit;
        }
        public async Task<bool> ConnectPaymentVnPayAsync(ConnectVnPayModel input)
        {
            try
            {
                string url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                string returnUrl = "https://localhost:7272";

                VnPayLibrary pay = new VnPayLibrary();

                pay.AddRequestData("vnp_Version", "2.1.0");
                pay.AddRequestData("vnp_Command", "pay");
                pay.AddRequestData("vnp_TmnCode", input.TmnCode);
                pay.AddRequestData("vnp_Amount", "1000000");
                pay.AddRequestData("vnp_BankCode", "");
                pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                pay.AddRequestData("vnp_CurrCode", "VND");
                pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress(_contextAccessor.HttpContext));
                pay.AddRequestData("vnp_Locale", "vn");
                pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang");
                pay.AddRequestData("vnp_OrderType", "other");
                pay.AddRequestData("vnp_ReturnUrl", returnUrl);
                pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

                string paymentUrl = pay.CreateRequestUrl(url, input.HashSecret);

                var check = CheckConnectPaymentAsync(paymentUrl);

                _webManageService.Close();

                if (check)
                {
                    var data = _mapper.Map<ConnectPayment>(input);
                    await _connectPaymentRepository.CreateAsync(data);
                    return check;
                }
                return check;
            }
            catch(FriendlyException ex)
            {
                _logger.LogError($"ConnectPaymentVnPay.Exception: {ex.Message}");
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_PaymentService, ex.Message);
            }
        }

        private bool CheckConnectPaymentAsync(string url)
        {
            try
            {
                _webManageService.Driver.Navigate().GoToUrl(url);

                // Tìm thẻ <img> thông qua XPath hoặc các phương thức khác
                var imageElement = _webManageService.Driver.FindElements(By.XPath("//img[@src='/paymentv2/images/graph/error.svg']"));


                if (imageElement.Any())
                {
                    return false;
                }
                return true;
            }
            catch(FriendlyException ex)
            {
                _logger.LogError($"CheckConnectPayment exception: {ex.Message}", url);
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_PaymentService, ex.Message);
            }
        }
    }
}
