
namespace InMotionGIT.Common.Helpers
{

    public class Values
    {

        // % insGetSetting: se toman los valore del registro
        public string insGetSetting(string Name, string DefValue, string Group = "")
        {
            string insGetSettingRet = default;
            var lclsConfig = new VisualTimeConfig();

            insGetSettingRet = lclsConfig.LoadSetting(Name, DefValue, Group);
            // UPGRADE_NOTE: Object lclsConfig may not be destroyed until it is garbage collected. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            lclsConfig = null;
            return insGetSettingRet;

        }

    }

}