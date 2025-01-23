using UnityEngine;
using Zenject;

public class MagicInstaller : MonoInstaller
{
    [SerializeField] private MagicKeyKombination _magicKeyKombination;

    public override void InstallBindings()
    {
        Container.Bind<MagicKeyKombination>().FromInstance(_magicKeyKombination);
    }

}
