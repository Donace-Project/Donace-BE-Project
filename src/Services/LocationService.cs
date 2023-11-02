using Donace_BE_Project.Constant;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Services;
using System.Device.Location;
using Newtonsoft.Json;

namespace Donace_BE_Project.Services
{
    public class LocationService : ILocationService
    {
        public ILogger<LocationService> _logger { get; set; }
        public LocationService(ILogger<LocationService> logger)
        {

            _logger = logger;

        }
        public string GetAreaAsync(double longitude, double latitude)
        {
            try
            {
                GeoCoordinate daNangLocation = new GeoCoordinate(16.047079, 108.202166);
                GeoCoordinate HaNoiLocation = new GeoCoordinate(21.0285, 105.8461);
                GeoCoordinate HCMocation = new GeoCoordinate(10.7769, 106.6860);

                GeoCoordinate yourLocation = new GeoCoordinate(latitude, longitude);

                double distanceDaNang = yourLocation.GetDistanceTo(daNangLocation);
                double distanceHCM = yourLocation.GetDistanceTo(HCMocation);
                double distanceHaNoi = yourLocation.GetDistanceTo(HaNoiLocation);

                if(distanceDaNang < 1000) 
                {
                    return "DaNang";
                }
                else if(distanceHaNoi < 1000)
                {
                    return "HaNoi";
                }
                else if(distanceHCM < 1000) 
                {
                    return "HCM";
                }

                return "VietNam";
            }
            catch (Exception ex)
            {
                _logger.LogError($"LocationService.Exception: {ex.Message}", JsonConvert.SerializeObject(new {longitude, latitude}));
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_LocationService, ex.Message);
            }
        }
    }
}
