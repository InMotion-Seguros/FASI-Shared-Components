using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace InMotionGIT.Common.Base
{

    [Serializable()]
    [XmlType(Namespace = "urn:InMotionGIT.UI.Model")]
    [XmlRoot(Namespace = "urn:InMotionGIT.UI.Model")]
    public class ActionCollection : Collection<Action>
    {

        public ActionCollection Clone()
        {
            var _actionCollection = new ActionCollection();
            Action newItem;

            foreach (Action ruleItem in this)
            {
                newItem = ruleItem.Clone();
                _actionCollection.Add(newItem);
            }

            return _actionCollection;
        }

    }

}