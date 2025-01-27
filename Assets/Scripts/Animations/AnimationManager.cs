using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AnimationManager {
    private BubblePool bubblePool;

    public AnimationManager() {
        bubblePool = ServiceLocator.Get<BubblePool>();
    }

    public void MoveAnimation(Transform moveTransform, Vector3 center) {
        float coeff = .1f;
        float duration = .25f;
        Vector3 direction = (moveTransform.position - center).normalized;
        Vector3 pos = moveTransform.position + direction * coeff;
        moveTransform.DOMove(pos, duration).SetLoops(2, LoopType.Yoyo);
    }

    public async Task PopAnimationSequentialAsync(List<Transform> bubbleTransforms) {
        Sequence sequence = DOTween.Sequence();
        float duration = 0.1f;

        foreach (var bubbleTransform in bubbleTransforms) {
            sequence.Append(bubbleTransform.DOScale(Vector3.one * 1.2f, duration * 0.3f)
                .SetEase(Ease.OutQuad))
                .Append(bubbleTransform.DOScale(Vector3.one * 0.8f, duration * 0.3f)
                .SetEase(Ease.InOutQuad))
                .Append(bubbleTransform.DOScale(Vector3.zero, duration * 0.4f)
                .SetEase(Ease.InBack)
                .OnComplete(() => {
                    bubbleTransform.DOKill();
                    bubblePool.ReturnBubble(bubbleTransform.GetComponent<Bubble>());
                })
            );
        }

        await sequence.AsyncWaitForCompletion();
    }

}