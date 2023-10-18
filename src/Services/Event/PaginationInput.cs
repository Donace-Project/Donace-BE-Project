using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Shared.Pagination;

namespace Donace_BE_Project.Services.Event
{
    internal class PaginationInput<T> : PaginationInput
    {
        private bool v;
        private object donace_BE_Project_Bad_EventService_Input_Success;
        private EventCreateInput model;
        private object value;

        public PaginationInput(bool v, object donace_BE_Project_Bad_EventService_Input_Success, EventCreateInput model, object value)
        {
            this.v = v;
            this.donace_BE_Project_Bad_EventService_Input_Success = donace_BE_Project_Bad_EventService_Input_Success;
            this.model = model;
            this.value = value;
        }
    }
}