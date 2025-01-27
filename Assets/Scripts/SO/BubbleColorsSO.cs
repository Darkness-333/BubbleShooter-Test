using UnityEngine;

[CreateAssetMenu(fileName ="BubbleColors")]
public class BubbleColorsSO : ScriptableObject
{
    [SerializeField] private Color[] colors;
    public Color[] GetColors() => colors;
}
