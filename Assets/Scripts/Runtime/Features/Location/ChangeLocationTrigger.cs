using UnityEngine;
using Zenject;

namespace Runtime.Features.Location
{
    [RequireComponent(typeof(Collider))]
    public class ChangeLocationTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform _nextPositionTransform;
        private LocationChanger _locationChanger;

        [Inject]
        private void Construct(LocationChanger locationChanger)
        {
            _locationChanger = locationChanger;
        }

        public void Interact()
        {
            _locationChanger.ChangeLocation(_nextPositionTransform);
        }
    }
}
