using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private PlayerInput _playerInput;
    
    public override void InstallBindings()
    {
        Container.Bind<PlayerInput>().FromInstance(_playerInput);
    }
}