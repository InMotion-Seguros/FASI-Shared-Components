using InMotionGIT.Common.Core.Helpers.BackOffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace InMotionGIT.Common.Core.Helpers;

public class MultiCompany
{
    #region Field

    public int Identification { get; set; }
    public string Name { get; set; }

    #endregion Field

    #region Method

    public static object GetUserInfo(short nCompanyId)
    {
        object GetUserInfoRet = default;
        InMotionGIT.Common.Domain.BackOffice.VisualTimeConfig clsConfig;
        string companyName = "";
        string companyUser = "";
        string companyPassword = "";
        var arrResult = new object[3];

        arrResult[0] = string.Empty;
        arrResult[1] = string.Empty;
        arrResult[2] = string.Empty;
        clsConfig = new InMotionGIT.Common.Domain.BackOffice.VisualTimeConfig();
        if (Helpers.BackOffice.VisualTimeConfigHandler.GetCompanySettings(nCompanyId, ref companyName, ref companyUser, ref companyPassword))
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
            sMultiCom = ValuesHandler.insGetSetting("MultiCompany", "No", "Database");
        }
        return string.Compare(sMultiCom, "Yes", true) == 0;
    }

    public static DataTable MultiCompanyList()
    {
        string companyName = string.Empty;
        int intIndex;

        // Definicion del DataTable
        var List = new DataTable("List");
        var id = new DataColumn("id");
        var name = new DataColumn("name");

        // Creacion del DataTable
        List.Columns.Add(id);
        List.Columns.Add(name);

        for (intIndex = 1; intIndex <= 20; intIndex++)
        {
            string argcompanyUser = "";
            string argcompanyPassword = "";
            if (VisualTimeConfigHandler.GetCompanySettings((short)intIndex, ref companyName, ref argcompanyUser, ref argcompanyPassword))
            {
                List.Rows.Add(intIndex.ToString(), companyName);
            }
        }

        return List;
    }

    #endregion Method
}