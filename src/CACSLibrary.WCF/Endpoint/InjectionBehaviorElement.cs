using System;
using System.ServiceModel.Configuration;

namespace CACSLibrary.WCF.Endpoint
{
    public class InjectionBehaviorElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new InjectionBehavior();
        }
        public override Type BehaviorType
        {
            get
            {
                return typeof(InjectionBehavior);
            }
        }
    }
}
