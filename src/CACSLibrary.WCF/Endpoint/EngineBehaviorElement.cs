using System;
using System.ServiceModel.Configuration;

namespace CACSLibrary.WCF.Endpoint
{
    public class EngineBehaviorElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new EngineBehavior();
        }
        public override Type BehaviorType
        {
            get
            {
                return typeof(EngineBehavior);
            }
        }
    }
}
