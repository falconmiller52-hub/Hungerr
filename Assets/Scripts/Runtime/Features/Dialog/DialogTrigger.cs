using FMODUnity;
using Ink.Runtime;
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

		[SerializeField] [Tooltip("Звук игрока при монологе")]
		private EventReference _playerMonologSound;

		private DialogSystem _dialogSystem;
		private Story _story;

		[Inject]
		public void Construct(DialogSystem dialogSystem)
		{
			_dialogSystem = dialogSystem;
		}

		private void Start()
		{
			_story = new Story(_storyJson.text);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponent<PlayerMovement>() != null)
			{
				if (_storyJson == null)
				{
					Debug.LogError($"{name}: Story JSON is missing");
					return;
				}

				_dialogSystem.StartStory(_story, _playerMonologSound, _isMonolog);
				
				if (_isOnce)
					Destroy(this.gameObject);
			}
		}
	}
}