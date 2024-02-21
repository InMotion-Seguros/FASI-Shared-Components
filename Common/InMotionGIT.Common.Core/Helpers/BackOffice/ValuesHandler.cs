using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMotionGIT.Common.Core.Helpers.BackOffice;

public static class ValuesHandler
{
    // % insGetSetting: se toman los valore del registro
    public static string insGetSetting(string Name, string DefValue, string Group = "")
    { 
        return VisualTimeConfigHandler.LoadSetting(Name, DefValue, Group);
    }
}
