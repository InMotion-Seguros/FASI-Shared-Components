using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace InMotionGIT.Common.Base
{

    [Serializable()]
    [XmlType(Namespace = "urn:InMotionGIT.UI.Model")]
    [XmlRoot(Namespace = "urn:InMotionGIT.UI.Model")]
    public abstract class Action
    {

        #region Public Properties

        [XmlAttribute()]
        [Browsable(false)]
        public string ControlName { get; set; }

        #endregion

        public abstract string Description();

        #region Functions

        public abstract Action Clone();

        #endregion

    }

}