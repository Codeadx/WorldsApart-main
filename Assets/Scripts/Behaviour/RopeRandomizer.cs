using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeRandomizer : MonoBehaviour
{    
    public GrapplingGun GrapplingGun;
    public List<Transform>[] spawnPoints = new List<Transform>[3];

    void Awake() {
        GrapplingGun = GetComponent<GrapplingGun>();
        spawnPoints[0] = new List<Transform>();
        spawnPoints[1] = new List<Transform>();
        spawnPoints[2] = new List<Transform>();
    }
    void Update() {
        for (int x = 0, y = 8, z = 16; x < 8 && y < 16 && z < 24; x++, y++, z++) {
            if (spawnPoints[0].Count < 8) {
                spawnPoints[0].Add(GrapplingGun.ropeShotVisualizerSpawnPoints[x]);
            }
            if (spawnPoints[1].Count < 8) {
                spawnPoints[1].Add(GrapplingGun.ropeShotVisualizerSpawnPoints[y]);
            }
            if (spawnPoints[2].Count < 8) {
                spawnPoints[2].Add(GrapplingGun.ropeShotVisualizerSpawnPoints[z]);
            }
        } 
    }
}
