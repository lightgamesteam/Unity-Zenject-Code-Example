using DG.Tweening;

public class DoTimer
{
    public DoTimer(float time, TweenCallback callback)
    {
        Sequence timer = DOTween.Sequence();
        timer.AppendInterval(time);
        timer.AppendCallback(callback);
    }
}
