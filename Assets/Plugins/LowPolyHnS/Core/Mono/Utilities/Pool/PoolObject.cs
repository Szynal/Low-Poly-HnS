using System.Collections;
using UnityEngine;

namespace LowPolyHnS.Pool
{
    [AddComponentMenu("LowPolyHnS/Pool/Pool Object")]
    public class PoolObject : MonoBehaviour
    {
        public const int INIT_COUNT = 20;
        public const float DURATION = 10f;

        // PROPERTIES: ---------------------------------------------------------

        public int initCount = INIT_COUNT;
        public float duration = DURATION;

        private IEnumerator coroutine;

        // PRIVATE METHODS: ----------------------------------------------------

        private void OnEnable()
        {
            coroutine = SetDisable();
            StartCoroutine(coroutine);
        }

        private void OnDisable()
        {
            CancelInvoke();
            StopCoroutine(coroutine);
        }

        private IEnumerator SetDisable()
        {
            WaitForSeconds wait = new WaitForSeconds(duration);
            yield return wait;

            gameObject.SetActive(false);
        }
    }
}