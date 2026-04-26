using FMODUnity;
using Ink.Runtime;
using NaughtyAttributes;
using Runtime.Features.Player.Movement;
using UnityEngine;
using Zenject;

namespace Runtime.Features._Story.Monolog
{
	[RequireComponent(typeof(Collider))]
	public class MonologTrigger : MonoBehaviour
	{
		[SerializeField] [Tooltip("Ставить если диалог/монолог должен проиграть один раз")]
		private bool _isOnce;

		[SerializeField] [Tooltip("JSON файл диалога")]
		private TextAsset _storyJson;

		[SerializeField] [Tooltip("Звук игрока при монологе")]
		private EventReference _playerMonologSound;

		private StorySystem _storySystem;
		private Story _story;

		[Inject]
		public void Construct(StorySystem storySystem)
		{
			_storySystem = storySystem;
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
				
				_story = new Story(_storyJson.text);
				
				_storySystem.StartStory(_story, _playerMonologSound, true);

				if (_isOnce)
					Destroy(this.gameObject);
			}
		}
		
		// Debug

		[Button]
		public void StartMonolog()
		{
			_story = new Story(_storyJson.text);
				
			_storySystem.StartStory(_story, _playerMonologSound, true);
		}
	}
}