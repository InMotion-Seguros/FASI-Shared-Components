
namespace InMotionGIT.Common.BackOffice
{

    public class Values
    {

        public string insGetSetting(string Name, string DefValue, string Group = "")
        {
            string insGetSettingRet = default;
            var lclsConfig = new VisualTimeConfig();

            insGetSettingRet = lclsConfig.LoadSetting(Name, DefValue, Group);
            lclsConfig = null;
            return insGetSettingRet;

        }

    }

}