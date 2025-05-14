using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddonUpdater.Models
{
    class LastUpdateAddon
    {
        public required string AddonName { get; set; } = string.Empty;

        public required string LastUpdate { get; set; } = string.Empty;
    }
}
