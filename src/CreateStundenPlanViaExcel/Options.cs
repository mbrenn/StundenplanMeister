using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateStundenPlanViaExcel
{
    internal class Options
    {
        [Value(0, MetaName = "Excel-File with Events")]
        public string InputExcel { get; set; }


        [Value(1, MetaName = "Resulting Html File with Calendar")]
        public string OutputHtml { get; set; }

    }
}
