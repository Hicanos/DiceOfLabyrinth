using System.Collections;
using UnityEngine;

public class BattleUILogButton : MonoBehaviour
{
    [SerializeField] RectTransform log;
    [SerializeField] float destTime;
    bool isOff = true;
    float currentRatio;

    IEnumerator openCoroutine;
    IEnumerator closeCoroutine;

    public void OnClickLog()
    {
        if(isOff)
        {
            isOff = false;

            if(closeCoroutine != null)
            {
                StopCoroutine(closeCoroutine);
            }

            openCoroutine = OpenLog();
            StartCoroutine(openCoroutine);
        }
        else
        {
            isOff = true;

            if(openCoroutine != null)
            {
                StopCoroutine(openCoroutine);
            }

            closeCoroutine = CloseLog();
            StartCoroutine(closeCoroutine);
        }
    }

    IEnumerator OpenLog()
    {
        float pastTime = currentRatio * destTime;
        float ratio;
        Vector3 scale = new Vector3(0,0,1);
        while(pastTime <= destTime)
        {            
            ratio = pastTime / destTime;
            scale.x = ratio;
            scale.y = ratio;
            currentRatio = ratio;

            log.localScale = scale;

            pastTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator CloseLog()
    {
        float pastTime = currentRatio * destTime;
        float ratio;
        Vector3 scale = new Vector3(1, 1, 1);
        while (pastTime >= 0)
        {
            ratio = pastTime / destTime;
            scale.x = ratio;
            scale.y = ratio;
            currentRatio = ratio;

            log.localScale = scale;

            pastTime -= Time.deltaTime;
            yield return null;
        }
    }
}
