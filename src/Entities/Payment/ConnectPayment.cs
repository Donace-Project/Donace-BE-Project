using Donace_BE_Project.Entities.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Donace_BE_Project.Entities.Payment
{
    public class ConnectPayment : BaseEntity
    {
        public string SecretKey { get; set; }

        public string Key { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sorted { get; set; } 
    }
}
