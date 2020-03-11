using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FearEffect.Puzzle
{
    public class FE_PipesController : MonoBehaviour, IMessageReciever
    {
        [SerializeField] FE_Lvl01_HotPipe   closeCornerPipe = null,
                                            middleCrossroadsPipe = null,
                                            nearValvePipe = null, 
                                            crossroadsRightClosePipe = null, 
                                            crossroadsRightFarPipe = null, 
                                            farEndPipe = null, 
                                            nearSafespotPipe = null, 
                                            nearSavePipe1 = null,
                                            nearSavePipe2 = null,
                                            nearSavePipe3 = null,
                                            closeDeadendPipe = null,
                                            farDeadendPipe = null;

        private bool usingFirstPattern = true;

        private void Start()
        {
            StartCoroutine(handleFirstPattern());
        }

        public void OnMessageRecieved(EMessage _incomingMsg)
        {
            if(_incomingMsg == EMessage.ChangeState)
            {
                StopAllCoroutines();
                usingFirstPattern = false;
                disableAllPipes();

                StartCoroutine(handleSecondPattern());
            }
        }

        private IEnumerator handleFirstPattern()
        {
            yield return new WaitForSeconds(1f);

            while (usingFirstPattern == true)
            {
                closeCornerPipe.SetState(true);
                nearSafespotPipe.SetState(true);

                yield return new WaitForSeconds(2f);

                middleCrossroadsPipe.SetState(true);
                crossroadsRightClosePipe.SetState(true);

                yield return new WaitForSeconds(3f);

                nearValvePipe.SetState(true);
                crossroadsRightFarPipe.SetState(true);
                farEndPipe.SetState(true);
                closeDeadendPipe.SetState(true);
                farDeadendPipe.SetState(true);
                nearSavePipe1.SetState(true);
                nearSavePipe2.SetState(true);
                nearSavePipe3.SetState(true);

                yield return new WaitForSeconds(3f);

                disableAllPipes();

                yield return new WaitForSeconds(3f);

                yield return null;
            }
        }

        private IEnumerator handleSecondPattern()
        {
            yield return new WaitForSeconds(1f);

            while(usingFirstPattern == false)
            {
                nearValvePipe.SetState(true);
                middleCrossroadsPipe.SetState(true);
                closeCornerPipe.SetState(true);

                yield return new WaitForSeconds(3f);

                closeDeadendPipe.SetState(true);
                farDeadendPipe.SetState(true);
                crossroadsRightClosePipe.SetState(true);
                
                yield return new WaitForSeconds(1f);

                farEndPipe.SetState(true);
                crossroadsRightFarPipe.SetState(true);

                nearSavePipe1.SetState(true);
                nearSavePipe2.SetState(true);
                nearSavePipe3.SetState(true);

                yield return new WaitForSeconds(1.5f);

                nearSafespotPipe.SetState(true);

                yield return new WaitForSeconds(3f);

                disableAllPipes();

                yield return new WaitForSeconds(3f);
                yield return null;
            }
        }

        private void disableAllPipes()
        {
            closeCornerPipe.SetState(false);
            nearSafespotPipe.SetState(false);
            middleCrossroadsPipe.SetState(false);
            crossroadsRightClosePipe.SetState(false);
            nearValvePipe.SetState(false);
            crossroadsRightFarPipe.SetState(false);
            farEndPipe.SetState(false);
            closeDeadendPipe.SetState(false);
            farDeadendPipe.SetState(false);
            nearSavePipe1.SetState(false);
            nearSavePipe2.SetState(false);
            nearSavePipe3.SetState(false);
        }
    }
}
