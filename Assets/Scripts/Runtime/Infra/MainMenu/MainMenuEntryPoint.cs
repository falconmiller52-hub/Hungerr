using Runtime.Common.Services.LoadingCurtain;
using UnityEngine;
using Zenject;

namespace Runtime.Infra.MainMenu
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        private ILoadingCurtain _loadingCurtain;

        [Inject]
        private void Construct(ILoadingCurtain loadingCurtain)
        {
            _loadingCurtain = loadingCurtain;
        }

        private void Start()
        {
            _loadingCurtain.Hide(0);
        }
    }
}
