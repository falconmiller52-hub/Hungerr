using System;
using System.Collections.Generic;
using Runtime.Features.Trade;
using UnityEngine;

namespace Runtime.Features._Story
{
	public class StoryTagSystem
	{
		public event Action OnTagStoryEnd;

		private TradeTagHandler _tradeTagHandler;

		public StoryTagSystem(TradeTagHandler tradeTagHandler)
		{
			_tradeTagHandler = tradeTagHandler;
		}

		public void ParseTag(List<string> tags)
		{
			foreach (var tag in tags)
			{
				string[] parts = tag.Trim().Split(':');

				if (parts.Length < 2)
				{
					Debug.LogError($"Invalid tag format - {tag}");
					return;
				}

				string key = parts[0];
				string value = parts[1];

				HandleTag(key, value);
			}
		}

		private void HandleTag(string key, string value)
		{
			switch (key)
			{
				case "Dialog":
				{
					// Это место исключение из правил общей архитектуры т.к так гораздо удобнее. 
					if (value == "Exit")
						OnTagStoryEnd?.Invoke();
					break;
				}
				case "Trade":
				{
					_tradeTagHandler.GetTagValue(value);
					break;
				}
			}
		}
	}
}