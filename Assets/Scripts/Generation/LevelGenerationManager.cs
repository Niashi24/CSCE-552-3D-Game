using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Csce552
{
    public class LevelGenerationManager : MonoBehaviour
    {
        public Transform player;

        public float tileLength = 2;

        public LevelSegment initialSegment;

        public LevelSegment[] randomSegments;

        public Vector3 forward = Vector3.back;

        public int tilesInFront = 20;
        public int tilesBehind = 5;

        public float radius = 2;
        
        // Runtime

        public Vector3 offset;

        public List<LevelSegment> activeSegments;

        public int currentTiles = 0;

        private int PlayerTile()
        {
            float playerPos = Vector3.Dot(player.position - offset, forward);
            return (int)(playerPos / tileLength);
        }

        void Start()
        {
            this.offset = player.position + Vector3.down * radius;
            this.currentTiles = 0;

            SpawnSegment(initialSegment);
            
            while (currentTiles < tilesInFront)
            {
                GenerateTile();
            }
        }

        void GenerateTile()
        {
            int tileIndex = (int)(Random.value * randomSegments.Length);
            var segment = randomSegments[tileIndex];
            SpawnSegment(segment);
        }

        private void SpawnSegment(LevelSegment segment)
        {
            Vector3 position = offset + currentTiles * tileLength * forward;
            segment = Instantiate(segment, position, Quaternion.identity, transform);
            currentTiles += segment.numTiles;
            activeSegments.Add(segment);
        }

        // Update is called once per frame
        void Update()
        {
            int playerTiles = PlayerTile();
            
            while (currentTiles - playerTiles < tilesInFront)
            {
                GenerateTile();
            }
        }
    }
}
