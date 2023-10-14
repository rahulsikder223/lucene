using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneWorker.DTOs
{
    public class ExportDTO
    {
        public string? PaperName { get; set; }
        public string? Description { get; set; }
        public double Score { get; set; }
    }
}
