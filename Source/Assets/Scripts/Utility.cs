using System;
using System.Collections;
using UnityEngine;

public class Utility : Singleton<Utility>
{
    [Header("Curve")]
    public AnimationCurve uiLerp;


    public void PlayAnimation(GameObject target, string name, Action done = null)
    {
        var animation = target.GetComponent<Animation>();
        animation.Stop();
        animation.Play(name);
        if (done != null)
        {
            StartCoroutine(WaitUntilAnimationFinished(animation, done));
        }
    }
    private IEnumerator WaitUntilAnimationFinished(Animation animation, Action callback)
    {
        while (animation.isPlaying)
        {
            yield return null;
        }
        callback?.Invoke();
        yield break;
    }
    public void LerpMoveUI(RectTransform rect, Vector3 destination, float duration)
    {
        StartCoroutine(DoLerpMoveUI(rect, destination, duration));
    }
    private IEnumerator DoLerpMoveUI(RectTransform rect, Vector3 destination, float duration)
    {
        float timer = 0;
        Vector3 start = rect.anchoredPosition;
        while (timer <= duration)
        {
            rect.anchoredPosition = Vector3.Lerp(start, destination, uiLerp.Evaluate(timer / duration));
            timer += Time.deltaTime;
            yield return null;
        }
        yield break;
    }
}
