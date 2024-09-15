using UnityEngine;
[RequireComponent (typeof(SpriteRenderer))]
public class Building : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [field: SerializeField] public int BuildingCost { get; private set; } = 15;
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer> ();
    }
    public void ChangeColor(Color color)
    {
        _spriteRenderer.color = color;
    }

}
