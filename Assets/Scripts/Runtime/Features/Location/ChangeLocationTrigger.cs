using System;
using Runtime.Features.Interactable;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Location
{
	/// <summary>
	/// Триггер смены локации, вешается на объект с коллайдером и если игрок завзаимодействует с этим объектом то сменится поз. игрока на nextPosition
	/// </summary>
	[RequireComponent(typeof(Collider))]
	public class ChangeLocationTrigger : MonoBehaviour, IInteractable
	{
		[SerializeField] private Transform _nextPositionTransform;
		[SerializeField] private LocationChangerData _locationChangerData;
		
		private LocationChanger _locationChanger;

		[Inject]
		private void Construct(LocationChanger locationChanger)
		{
			_locationChanger = locationChanger;
		}

		public void Interact()
		{
			_locationChanger.ChangeLocation(_nextPositionTransform, _locationChangerData);
		}
	}
}