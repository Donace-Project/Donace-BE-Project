using AutoMapper;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Entities.Payment;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.VNPay;
using Donace_BE_Project.Shared;
using OpenQA.Selenium;

namespace Donace_BE_Project.Services
{
    public class PaymentService : IPaymentService
    {
        public readonly ILogger<PaymentService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConnectPaymentRepository _connectPaymentRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly Lazy<IWebManageService> _webManageService;
        public PaymentService(ILogger<PaymentService> logger,
                              IConfiguration configuration,
                              IHttpContextAccessor contextAccessor,
                              IConnectPaymentRepository connectPaymentRepository,
                              IMapper mapper,
                              IUnitOfWork unit,
                              IUserProvider userProvider,
                              Lazy<IWebManageService> webManageService)
        {
            _logger = logger;
            _configuration = configuration;
            _contextAccessor = contextAccessor;
            _connectPaymentRepository = connectPaymentRepository;
            _mapper = mapper;
            _unitOfWork = unit;
            _userProvider = userProvider;
            _webManageService = webManageService;
        }
        public async Task<ResponseModel<bool>> ConnectPaymentVnPayAsync(ConnectVnPayModel input)
        {
            try
            {
                var checkExist = await _connectPaymentRepository.GetByUserAsync(_userProvider.GetUserId());

                if (checkExist is not null)
                {
                    return new ResponseModel<bool>
                    {
                        Success = true,
                        Code = "Connected",
                        Result = true,
                    };
                }

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

                if (check)
                {
                    var data = _mapper.Map<ConnectPayment>(input);
                    await _connectPaymentRepository.CreateAsync(data);
                    await _unitOfWork.SaveChangeAsync();
                    return new ResponseModel<bool>
                    {
                        Success = true,
                        Code = "Connected",
                        Result = true,
                    };
                }

                return new ResponseModel<bool>
                {
                    Success = false,
                    Message = "Không thể liên kết VNPAY! Vui lòng kiểm tra lại key",
                    Result = false,
                };
            }
            catch (FriendlyException ex)
            {
                _logger.LogError($"ConnectPaymentVnPay.Exception: {ex.Message}");
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_PaymentService, ex.Message);
            }
        }

        public async Task<ConnectVnPayModel> GetConnectAsync()
        {
            try
            {
                var userId = _userProvider.GetUserId();
                var data = await _connectPaymentRepository.GetByUserAsync(userId);

                if (data is null)
                {
                    return null;
                }

                return _mapper.Map<ConnectVnPayModel>(data);
            }
            catch (FriendlyException ex)
            {
                _logger.LogError($"GetConenct exception: {ex.Message}", _userProvider.GetUserId());
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_PaymentService, ex.Message);
            }
        }

        private bool CheckConnectPaymentAsync(string url)
        {
            try
            {
                _webManageService.Value.Driver.Navigate().GoToUrl(url);
                // Tìm thẻ <img> thông qua XPath hoặc các phương thức khác
                var imageElement = _webManageService.Value.Driver.FindElements(By.XPath("//img[@src='/paymentv2/images/graph/error.svg']"));
                _webManageService.Value.Close();
                return !imageElement.Any();
            }
            catch (FriendlyException ex)
            {
                _logger.LogError($"CheckConnectPayment exception: {ex.Message}", url);
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_PaymentService, ex.Message);
            }
        }
    }
}
