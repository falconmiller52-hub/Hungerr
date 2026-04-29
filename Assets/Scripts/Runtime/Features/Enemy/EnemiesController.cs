using System;
using System.Collections.Generic;
using Runtime.Common.Enums;
using Runtime.Common.Services.EventBus;
using Runtime.Features.Enemy.Thin;
using Runtime.Features.Enemy.Thin.States;
using Unity.AI.Navigation;
using UnityEngine;
using Zenject;

namespace Runtime.Features.Enemy
{
	/// <summary>
	/// скрипт который инитит всех врагов на сцене, задает их таргета - игрока
	/// </summary>
	public class EnemiesController : MonoBehaviour
	{
		[SerializeField] private NavMeshSurface _navMeshSurface;
		[SerializeField] private ThinEnemyAI _thinAIPrefab;

		private Dictionary<ThinEnemyAI, ThinSpawnPoint> _enemiesMap;
		private DiContainer _container;
		private EventBus _eventBus;

		[Inject]
		private void Construct(DiContainer diContainer, EventBus eventBus)
		{
			_container = diContainer;
			_eventBus = eventBus;
		}

		private void OnEnable()
		{
			_eventBus.Subscribe(EChangeLocation.ChangeLocationTriggered, SetAllEnemiesToPatrolState);
		}

		private void OnDisable()
		{
			_eventBus.Unsubscribe(EChangeLocation.ChangeLocationTriggered, SetAllEnemiesToPatrolState);
		}

		public void Init(GameObject targetPlayer)
		{
			if (_navMeshSurface == null)
				_navMeshSurface = FindAnyObjectByType<NavMeshSurface>();

			if (_navMeshSurface.navMeshData == null)
				_navMeshSurface.BuildNavMesh();

			if (targetPlayer == null)
			{
				Debug.LogError("EnemiesController::Init() Player is null");
				return;
			}

			_enemiesMap = new Dictionary<ThinEnemyAI, ThinSpawnPoint>();

			var thinSpawnPoints = FindObjectsByType<ThinSpawnPoint>(FindObjectsSortMode.None);

			if (thinSpawnPoints == null)
			{
				Debug.LogError("EnemiesController::Init() No spawnPoints found");
				return;
			}

			foreach (var spawnPoint in thinSpawnPoints)
			{
				ThinEnemyAI thinAi =
								_container.InstantiatePrefabForComponent<ThinEnemyAI>(_thinAIPrefab,
												spawnPoint.transform);

				thinAi.Agent.Warp(spawnPoint.transform.position);

				thinAi.Init(targetPlayer, spawnPoint.PatrolPoints);
				thinAi.ChangeState<PatrolState>();

				_enemiesMap.Add(thinAi, spawnPoint);
			}
		}

		public void SetAllEnemiesToSpawnPoint()
		{
			ForEachEnemy((ai) =>
			{
				ai.Agent.Warp(_enemiesMap[ai].transform.position);
				ai.ChangeState<PatrolState>();
			});
		}

		private void SetAllEnemiesToPatrolState()
		{
			ForEachEnemy((ai) => { ai.ChangeState<PatrolState>(); });
		}

		private void ForEachEnemy(Action<ThinEnemyAI> action)
		{
			if (_enemiesMap == null)
			{
				Debug.LogError("EnemiesController::SetAllEnemiesToPatrol() Enemies is null");
				return;
			}

			foreach (var ai in _enemiesMap.Keys)
			{
				action?.Invoke(ai);
			}
		}
	}
}