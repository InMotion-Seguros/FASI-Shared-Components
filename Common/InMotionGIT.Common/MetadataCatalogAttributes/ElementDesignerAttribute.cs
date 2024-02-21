using System;

namespace InMotionGIT.Common.Attributes
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ElementDesignerAttribute : Attribute
    {

        private int _Language = 1;
        private string _Caption;
        private string _ToolTip;

        public int Language
        {
            get
            {
                return _Language;
            }
        }

        public string Caption
        {
            get
            {
                return _Caption;
            }
        }

        public string ToolTip
        {
            get
            {
                return _ToolTip;
            }
        }

        protected ElementDesignerAttribute(int language, string caption)
        {
            _Language = language;
            _Caption = caption;
        }

        protected ElementDesignerAttribute(int language, string caption, string toolTip)
        {
            _Language = language;
            _Caption = caption;
            _ToolTip = toolTip;
        }

    }

}