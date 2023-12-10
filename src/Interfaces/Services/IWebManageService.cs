using OpenQA.Selenium;

namespace Donace_BE_Project.Interfaces.Services
{
    public interface IWebManageService : IDisposable
    {
        public IWebDriver Driver { get; set; }
        void Close();
    }
}
