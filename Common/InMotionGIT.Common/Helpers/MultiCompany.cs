using System.Data;

namespace InMotionGIT.Common.Helpers
{

    public class MultiCompany
    {

        #region Field

        public int Identification { get; set; }
        public string Name { get; set; }

        #endregion

        #region Method

        public static object GetUserInfo(short nCompanyId)
        {
            object GetUserInfoRet = default;
            VisualTimeConfig clsConfig;
            string companyName = "";
            string companyUser = "";
            string companyPassword = "";
            var arrResult = new object[3];

            arrResult[0] = string.Empty;
            arrResult[1] = string.Empty;
            arrResult[2] = string.Empty;
            clsConfig = new VisualTimeConfig();
            if (clsConfig.GetCompanySettings(nCompanyId, ref companyName, ref companyUser, ref companyPassword))
            {
                arrResult[0] = companyName;
                arrResult[1] = companyUser;
                arrResult[2] = companyPassword;
            }

            GetUserInfoRet = arrResult;
            return GetUserInfoRet;
        }

        public static bool IsMultiCompany()
        {
            string sMultiCom = string.Empty;
            {
                var withBlock = new Values();
                sMultiCom = withBlock.insGetSetting("MultiCompany", "No", "Database");
            }
            return string.Compare(sMultiCom, "Yes", true) == 0;
        }

        public static DataTable MultiCompanyList()
        {
            VisualTimeConfig clsConfig;
            string companyName = string.Empty;
            int intIndex;

            // Definicion del DataTable
            var List = new DataTable("List");
            var id = new DataColumn("id");
            var name = new DataColumn("name");

            // Creacion del DataTable
            List.Columns.Add(id);
            List.Columns.Add(name);

            clsConfig = new VisualTimeConfig();

            for (intIndex = 1; intIndex <= 20; intIndex++)
            {
                string argcompanyUser = "";
                string argcompanyPassword = "";
                if (clsConfig.GetCompanySettings((short)intIndex, ref companyName, ref argcompanyUser, ref argcompanyPassword))
                {
                    List.Rows.Add(intIndex.ToString(), companyName);
                }
            }

            return List;
        }

        #endregion

    }

}