using Donace_BE_Project.Interfaces.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;

namespace Donace_BE_Project.Services
{
    public class WebManageService : IWebManageService
    {
        public IWebDriver Driver { get; set; }

        public WebManageService()
        {
            Driver = new EdgeDriver();
        }

        public void Close()
        {
            Driver.Quit();
        }
    }
}
