using UnityEngine;
using Zenject;

namespace _GAME._1_Scripts.INK
{
	[RequireComponent(typeof(Collider))]
	public class DialogTrigger : MonoBehaviour
	{
		[SerializeField] [Tooltip("Ставить если диалог/монолог должен проиграть один раз")] 
		private bool _isOnce;
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
			if (other.CompareTag("Player"))
			{
				_dialogSystem.StartStory(_storyJson);
				if (_isOnce)
					Destroy(this.gameObject);
			}
		}
	}
}