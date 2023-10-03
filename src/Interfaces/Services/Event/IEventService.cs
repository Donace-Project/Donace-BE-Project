using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;

namespace Donace_BE_Project.Interfaces.Services.Event;
public interface IEventService
{
    Task<EventFullOutput> CreateAsync(EventCreateInput input);
}