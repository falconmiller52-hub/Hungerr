using Runtime.Features.Player.Movement;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Dialog
{
	[RequireComponent(typeof(Collider))]
	public class DialogTrigger : MonoBehaviour
	{
		[SerializeField] [Tooltip("Ставить если диалог/монолог должен проиграть один раз")]
		private bool _isOnce;

		[SerializeField] [Tooltip("True если должен начаться монолог")]
		private bool _isMonolog = true;

		[SerializeField] [Tooltip("JSON файл диалога")]
		private TextAsset _storyJson;

		private DialogSystem _dialogSystem;

		[Inject]
		public void Construct(DialogSystem dialogSystem)
		{
			_dialogSystem = dialogSystem;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.TryGetComponent<PlayerMovement>(out var playerMovement))
			{
				if (_storyJson == null)
				{
					Debug.LogError($"{name}: Story JSON is missing");
					return;
				}

				_dialogSystem.StartStory(_storyJson, _isMonolog);
				if (_isOnce)
					Destroy(this.gameObject);
			}
		}
	}
}