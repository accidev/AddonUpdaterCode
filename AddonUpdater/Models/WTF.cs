using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddonUpdater.Models
{
    class WTF
    {
        public required string Account { get; set; } = string.Empty;

        public required List<Realms> Realms { get; set; } = new();

    }
}
