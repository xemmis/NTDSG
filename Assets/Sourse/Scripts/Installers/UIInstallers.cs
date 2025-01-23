using UnityEngine;
using Zenject;

public class UIInstallers : MonoInstaller
{
    [SerializeField] private Wallet _wallet;

    public override void InstallBindings()
    {
        Container.Bind<Wallet>().FromInstance(_wallet);
    }
}
