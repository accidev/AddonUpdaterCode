using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddonUpdater.Models
{
    class FileCustom
    {
        public required string Path { get; set; } = string.Empty;
        public DateTime DateTimeCreate { get; set; }
    }
}
