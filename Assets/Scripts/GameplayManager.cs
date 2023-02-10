using System;

namespace AFSInterview
{
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class GameplayManager : MonoBehaviour
    {
        [Header("Prefabs")] 
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject simpleTowerPrefab;
        [SerializeField] private GameObject burstTowerPrefab;

        [Header("Settings")] 
        [SerializeField] private Vector2 boundsMin;
        [SerializeField] private Vector2 boundsMax;
        [SerializeField] private float enemySpawnRate;
        [SerializeField] private LayerMask towerSpawnLayer;

        [Header("UI")] 
        [SerializeField] private TextMeshProUGUI enemiesCountText;
        [SerializeField] private TextMeshProUGUI scoreText;
        
        private List<Enemy> enemies;
        private float enemySpawnTimer;
        private int score;
        private Camera mainCamera;
        private Action onScoreChange;
        private Action onEnemiesCountChange;
        
        private const float MaxRaycastDistance = 35f;

        
        private void Awake()
        {
            enemies = new List<Enemy>();
            mainCamera = Camera.main;
        }

        private void Start()
        {
            UpdateScore();
            
            onScoreChange += UpdateScore;
            onEnemiesCountChange += UpdateEnemiesCount;
        }

        private void Update()
        {
            enemySpawnTimer -= Time.deltaTime;

            if (enemySpawnTimer <= 0f)
            {
                SpawnEnemy();
                enemySpawnTimer = enemySpawnRate;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (TryObtainSpawnPosition(out var spawnPosition))
                    SpawnTower(spawnPosition, simpleTowerPrefab);
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                if (TryObtainSpawnPosition(out var spawnPosition))
                    SpawnTower(spawnPosition, burstTowerPrefab);
            }
        }

        private void SpawnEnemy()
        {
            var position = new Vector3(Random.Range(boundsMin.x, boundsMax.x), enemyPrefab.transform.position.y, Random.Range(boundsMin.y, boundsMax.y));
            
            var enemy = Instantiate(enemyPrefab, position, Quaternion.identity).GetComponent<Enemy>();
            enemy.OnEnemyDied += Enemy_OnEnemyDied;
            enemy.Initialize(boundsMin, boundsMax);

            enemies.Add(enemy);
            
            onEnemiesCountChange?.Invoke();
        }

        private void Enemy_OnEnemyDied(Enemy enemy)
        {
            enemies.Remove(enemy);
            score++;
            
            onScoreChange?.Invoke();
            onEnemiesCountChange?.Invoke();
        }

        private bool TryObtainSpawnPosition(out Vector3 spawnPosition)
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, MaxRaycastDistance, towerSpawnLayer.value))
            {
                spawnPosition = hit.point;
                spawnPosition.y = simpleTowerPrefab.transform.position.y;
                return true;
            }
            
            spawnPosition = Vector3.zero;

            return false;
        }

        private void SpawnTower(Vector3 position, GameObject towerPrefab)
        {
            var tower = Instantiate(towerPrefab, position, Quaternion.identity).GetComponent<Tower>();
            tower.Initialize(enemies);
        }
        
        private void UpdateScore()
        {
            scoreText.text = $"Score: {score}";
        }

        private void UpdateEnemiesCount()
        {
            enemiesCountText.text = $"Enemies: {enemies.Count}";
        }
    }
}