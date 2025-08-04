using UnityEngine;
using System.Collections;

public class BattleUILogButton : AbstractBattleButton
{
    [SerializeField] RectTransform scrollView;    
    [SerializeField] float destTime;
    bool isOff = true;
    float currentRatio;

    IEnumerator openCoroutine;
    IEnumerator closeCoroutine;

    public override void Setting()
    {
        
    }

    public override void OnOffButton(DetailedTurnState state)
    {        
    }

    public override void OnPush()
    {
        if (isOff)
        {
            isOff = false;

            if (closeCoroutine != null)
            {
                StopCoroutine(closeCoroutine);
            }

            openCoroutine = OpenLog();
            StartCoroutine(openCoroutine);
        }
        else
        {
            isOff = true;

            if (openCoroutine != null)
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

            scrollView.localScale = scale;

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

            scrollView.localScale = scale;

            pastTime -= Time.deltaTime;
            yield return null;
        }
    }
}
