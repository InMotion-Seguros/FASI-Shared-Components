using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMotionGIT.Common.Domain.Configuration;

public class FASIConfiguration
{
    public Security Security { get; set; }
    public Mail Mail { get; set; }
    public General.General General { get; set; }
    public Dictionary<string, string> AppSetting { get; set; }

    public Dictionary<string, ConnectionStrings> ConnectionStrings { get; set; }
}
