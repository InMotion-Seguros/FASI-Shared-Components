using System;

namespace InMotionGIT.Common.Attributes
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks></remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class EntityDesignerAttribute : Attribute
    {

        private int _Language = 1;
        private string _Title;

        public int Language
        {
            get
            {
                return _Language;
            }
        }

        public string Title
        {
            get
            {
                return _Title;
            }
        }

        protected EntityDesignerAttribute(int language, string title)
        {
            _Language = language;
            _Title = title;
        }

    }

}