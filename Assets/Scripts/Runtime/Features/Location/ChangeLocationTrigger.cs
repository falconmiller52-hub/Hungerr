using Runtime.Features.Interactable;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Location
{
    /// <summary>
    /// триггер смены локации, вешается на объект с коллайдером и если игрок завзаимодействует с этим объектом то сменится поз. игрока на nextPosition
    /// </summary>
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
