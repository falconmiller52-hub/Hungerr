using TMPro;
using UnityEngine;
using Zenject;

namespace Runtime.Features._Story.Monolog.View
{
	public class MonologUi : MonoBehaviour
	{
		[SerializeField] private GameObject _monologPanel;
		[SerializeField] private TextMeshProUGUI _monologTmp;

		private StorySystem _storySystem;

		[Inject]
		public void Construct(StorySystem storySystem)
		{
			_storySystem = storySystem;
		}

		private void OnEnable()
		{
			_storySystem.OnMonologStoryStarted += ShowMonolog;
			_storySystem.OnNewStoryLine += SetText;
			_storySystem.OnStoryEnded += HideMonolog;
		}

		private void OnDisable()
		{
			_storySystem.OnMonologStoryStarted -= ShowMonolog;
			_storySystem.OnNewStoryLine -= SetText;
		}

		private void SetText(string text)
			=> _monologTmp.text = text;

		private void ShowMonolog()
		{
			_monologTmp.text = "";

			_monologPanel.SetActive(true);
		}
		
		private void HideMonolog()
		{
			_monologPanel.SetActive(false);
			_monologTmp.text = "";
		}
	}
}