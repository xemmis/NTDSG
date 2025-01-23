using UnityEngine;

public class FireFlyMagicReaction : MonoBehaviour
{
    [SerializeField] private GameObject _fireFlyLight;
    [SerializeField] private DarkMagicFabric _darkFabric;
    [SerializeField] private HolyMagicFabric _lightFabric;
    [SerializeField] private ISpell _spellComp;    

    private void Start()
    {
        _fireFlyLight.SetActive(true);

        Controller.VoiceDarkMagic += DarkMagicFromFireFly;
        Controller.VoicelightMagic += LightMagicFromFireFly;
    }

    private void Update()
    {
        if(Input.GetMouseButton(1) && _spellComp != null) 
        {
            CastSpell();
        }
    }

    private void OnDisable()
    {
        Controller.VoiceDarkMagic -= DarkMagicFromFireFly;
        Controller.VoicelightMagic -= LightMagicFromFireFly;
    }

    private void DarkMagicFromFireFly(string name)
    {
        if (_spellComp != null)
        {
            _spellComp.SpellDeactivate();
            _spellComp = null;

        }

        _spellComp = _darkFabric.SpelInit(name, transform);        
        _fireFlyLight.SetActive(false);
    }

    private void LightMagicFromFireFly(string name)
    {
        if (_spellComp != null)
        {
            _spellComp.SpellDeactivate();
            _spellComp = null;
        }
        _spellComp = _lightFabric.SpelInit(name, transform);
        _fireFlyLight.SetActive(false);
    }

    private void CastSpell()
    {
        _spellComp.SpellActivate();
        _spellComp = null;
        _fireFlyLight.SetActive(true);
    }

}
