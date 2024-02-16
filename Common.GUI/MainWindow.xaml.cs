using InMotionGIT.Common;
using InMotionGIT.Common.Proxy;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Common.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                method1();
                //MultiCompanyListWithFormat();

                var Id = "XXXXX";

                var withBlock = new DataManagerFactory("select count(*) from usermember where USERMEMBER.ISANONYMOUS = 0", "USERMEMBER", "FrontOfficeConnectionString", 2);
                dynamic count = "";
                withBlock.Id = Id;
                count = withBlock.QueryExecuteScalar<Int32>();
                //count = withBlock.QueryExecuteScalarToInteger();
                //count = withBlock.QueryExecuteScalarToDecimal();
                //withBlock = new DataManagerFactory("select lastactivitydate from usermember where lower(usernamelow) = 'admin' and rownum = 1", "USERMEMBER", "FrontOfficeConnectionString");
                //count = withBlock.QueryExecuteScalarToDate();
                //withBlock = new DataManagerFactory("select email from usermember where lower(usernamelow) = 'admin' and rownum = 1", "USERMEMBER", "FrontOfficeConnectionString");
                //count = withBlock.QueryExecuteScalarToString();

                //withBlock = new DataManagerFactory("select * from usermember where USERMEMBER.ISANONYMOUS = 0", "USERMEMBER", "FrontOfficeConnectionString", 1);
                var users = withBlock.QueryExecuteToTable();
                ////var users = withBlock.QueryExecuteToTableJSON();
                ////count = withBlock.Check();
                ////count = withBlock.CommandExecute();
                ////withBlock.CommandExecuteAsynchronous() ;
                ////withBlock.CompanyIdSelect();
                ////count = withBlock.ConnectionStringAll("1");
                ////count = withBlock.ConnectionStringGet("FrontOfficeConnectionString");
                ////count = withBlock.ConnectionStringUserAndPassword("FrontOfficeConnectionString");
                ////count = withBlock.DataStructure("select * from usermember where USERMEMBER.ISANONYMOUS = 0") ;
                //count = withBlock.DataStructure("usermember");
            }
            catch (Exception ex)
            {
                //throw;
            }
        }

        public void method1()
        {
            method2();
        }

        public void method2()
        {
            method3();
        }

        public void method3()
        {
            LastaAtack();
        }

        public string LastaAtack()
        {
            var methodName = System.Reflection.MethodBase.GetCurrentMethod().Name.ToLower();
            var stackTrace = new StackTrace();
            var stackFrames = stackTrace.GetFrames();

            var item = stackFrames.Select((elem, index) => new { elem, index })
                        .First(p => ((StackFrame)p.elem).GetMethod().Name.ToLower().Equals(methodName)).index;

            return "";
        }

        public static DataTable MultiCompanyListWithFormat()
        {
            var clsConfig = new VisualTimeConfig();
            string companyName = string.Empty;
            short intIndex;

            //Definición del DataTable
            var result = new DataTable("List");
            var id = new DataColumn("id");
            var name = new DataColumn("name");

            //Creación del DataTable
            result.Columns.Add(id);
            result.Columns.Add(name);

            for (intIndex = 1; (intIndex <= 20); intIndex++)
            {
                var value = "";
                if (clsConfig.GetCompanySettings(intIndex, ref companyName, ref value, ref value))
                {
                    result.Rows.Add(intIndex.ToString(), companyName);
                }
            }

            return result;
        }

        // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
        /// <remarks/>
        [System.SerializableAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
        [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
        public partial class company
        {
            private byte idField;

            private string nameField;

            private string userField;

            private string passwordField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public byte id
            {
                get
                {
                    return this.idField;
                }
                set
                {
                    this.idField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string name
            {
                get
                {
                    return this.nameField;
                }
                set
                {
                    this.nameField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string user
            {
                get
                {
                    return this.userField;
                }
                set
                {
                    this.userField = value;
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string password
            {
                get
                {
                    return this.passwordField;
                }
                set
                {
                    this.passwordField = value;
                }
            }
        }
    }
}