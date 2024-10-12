using UnityEngine;
[RequireComponent (typeof(SpriteRenderer))]
public class Building : MonoBehaviour
{
    [SerializeField] private protected SpriteRenderer _spriteRenderer;
    [SerializeField] private protected BuildSize _buildSize;

    [field: SerializeField] public int LevelOfBuilding { get; private set; }
    [field: SerializeField] private protected bool IsPlaced;
    [field: SerializeField] public NavigationBar _navBar { get; private set; }
    [field: SerializeField] public int BuildCost { get; private set; }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer> ();
    }

    public virtual bool GetCondition()
    {
        return IsPlaced;
    }

    public virtual void TakeNavigationBar(NavigationBar NavBar)
    {
        _navBar = NavBar;
    }

    public virtual void ChangeCondition(bool condition)
    {
        if (condition)
        {
           _buildSize.Placed();
        }
        IsPlaced = condition;
    }

    public virtual void ChangeColor(Color color)
    {
        _spriteRenderer.color = color;
    }
}
