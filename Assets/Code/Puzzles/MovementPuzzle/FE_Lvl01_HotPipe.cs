using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FearEffect.Puzzle
{
    public class FE_Lvl01_HotPipe : FE_DamageZone
    {
        [Header("Properties native for HotPipe")]
        [SerializeField] Material inactiveMat = null;
        [SerializeField] Material activeMat = null;

        private MeshRenderer meshRenderer;

        protected override void Awake()
        {
            base.Awake();

            meshRenderer = GetComponent<MeshRenderer>();
        }

        protected override void handleShowingRepresentation(bool _show)
        {
            if(_show == true)
            {
                meshRenderer.material = activeMat;
            }
            else
            {
                meshRenderer.material = inactiveMat;
            }
        }
    }
}
