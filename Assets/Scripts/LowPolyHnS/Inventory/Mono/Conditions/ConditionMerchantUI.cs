using LowPolyHnS.Core;
using UnityEngine;

namespace LowPolyHnS.Inventory
{
    [AddComponentMenu("")]
    public class ConditionMerchantUI : ICondition
    {
        public enum State
        {
            IsOpen,
            IsClosed
        }

        public State merchant = State.IsOpen;

        public override bool Check(GameObject target)
        {
            switch (merchant)
            {
                case State.IsOpen:
                    return MerchantUIManager.IsMerchantOpen();

                case State.IsClosed:
                    return !MerchantUIManager.IsMerchantOpen();
            }

            return false;
        }

#if UNITY_EDITOR

        public static new string NAME = "Inventory/Merchant UI";
        public const string CUSTOM_ICON_PATH = "Assets/Content/Icons/Inventory/Conditions/";

        private const string NODE_TITLE = "Merchant UI {0}";

        public override string GetNodeTitle()
        {
            return string.Format(NODE_TITLE, merchant);
        }

#endif
    }
}