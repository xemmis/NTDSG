using UnityEngine;
[RequireComponent (typeof(SpriteRenderer))]
public class Building : MonoBehaviour
{
    [SerializeField] private protected SpriteRenderer _spriteRenderer;
    [field: SerializeField] public int LevelOfBuilding { get; private set; }
    [SerializeField] private protected bool _isPlacing;
    [SerializeField] private protected NavigationBar _navBar;
    [field: SerializeField] public int BuildCost { get; private set; }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer> ();
    }

    public virtual void TakeNavigationBar(NavigationBar NavBar)
    {
        _navBar = NavBar;
    }

    public virtual void ChangeCondition(bool condition)
    {
        _isPlacing = condition;
    }

    public virtual void ChangeColor(Color color)
    {
        _spriteRenderer.color = color;
    }



}
