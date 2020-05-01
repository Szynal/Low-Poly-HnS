using System.Collections;
using LowPolyHnS.Core;
using UnityEngine;
using UnityEngine.UI;

namespace LowPolyHnS.Messages
{
    [AddComponentMenu("LowPolyHnS/Managers/SimpleMessageManager", 100)]
    public class SimpleMessageManager : Singleton<SimpleMessageManager>
    {
        private const string CANVAS_ASSET_PATH = "LowPolyHnS/Messages/SimpleMessage";

        private static int ANIMATOR_HASH_SHOW = -1;
        private static int ANIMATOR_HASH_HIDE = -1;
        private static int ANIMATOR_HASH_OPEN = -1;

        private static bool MESSAGE_STATE_OPEN;

        // PROPERTIES: -------------------------------------------------------------------------------------------------

        private Animator messageAnimator;
        private Text text;

        // INITIALIZE: -------------------------------------------------------------------------------------------------

        protected override void OnCreate()
        {
            EventSystemManager.Instance.Wakeup();

            ANIMATOR_HASH_SHOW = Animator.StringToHash("Show");
            ANIMATOR_HASH_HIDE = Animator.StringToHash("Hide");
            ANIMATOR_HASH_OPEN = Animator.StringToHash("IsOpen");

            DatabaseGeneral general = DatabaseGeneral.Load();
            GameObject prefab = general.prefabSimpleMessage;
            if (prefab == null) prefab = Resources.Load<GameObject>(CANVAS_ASSET_PATH);

            GameObject instance = Instantiate(prefab, transform);
            messageAnimator = instance.GetComponentInChildren<Animator>();
            text = instance.GetComponentInChildren<Text>();
        }

        protected override bool ShouldNotDestroyOnLoad()
        {
            return false;
        }

        // PUBLIC METHODS: ---------------------------------------------------------------------------------------------

        public void ShowText(string text, Color color)
        {
            this.text.text = text;
            this.text.color = color;
            MESSAGE_STATE_OPEN = true;
            messageAnimator.SetTrigger(ANIMATOR_HASH_SHOW);
            messageAnimator.SetBool(ANIMATOR_HASH_OPEN, true);
        }

        public void HideText()
        {
            MESSAGE_STATE_OPEN = false;
            StartCoroutine(HideTextDelayed());
        }

        // PRIVATE METHODS: --------------------------------------------------------------------------------------------

        private IEnumerator HideTextDelayed()
        {
            YieldInstruction waitForSeconds = new WaitForSeconds(0.1f);
            yield return waitForSeconds;

            if (!MESSAGE_STATE_OPEN)
            {
                messageAnimator.SetTrigger(ANIMATOR_HASH_HIDE);
                messageAnimator.SetBool(ANIMATOR_HASH_OPEN, false);
            }
        }
    }
}