using System.Collections.Generic;
using Runtime.Common.Extensions;
using UnityEngine;

namespace Runtime.Features.Enemy.Domovoi.Patterns
{
	// паттерн при котором на рандомных объектах из списка появляется рандомный спрайт
	public class SetRandomImageToRandomObjectPattern : DomovoiPattern
	{
		[SerializeField] private MeshRenderer[] _randomMeshRendererObjects;
		[SerializeField] private Material _materialSample;
		[SerializeField] private List<Texture> _randomSprites;

		[ContextMenu("Trigger")]
		public override void Trigger()
		{
			base.Trigger();

			foreach (var image in _randomMeshRendererObjects)
			{
				if (image != null)
				{
					Material newMaterial = new Material(_materialSample);

					newMaterial.mainTexture = _randomSprites.Random();
					image.material = newMaterial;

					image.gameObject.SetActive(true);
				}
			}
		}

		[ContextMenu("Clear")]
		public override void Clear()
		{
			base.Clear();

			foreach (var image in _randomMeshRendererObjects)
			{
				if (image != null)
					image.gameObject.SetActive(false);
			}
		}
	}
}