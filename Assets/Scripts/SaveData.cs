using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData {

    // Player 
    public float[] position = new float[3];
    public bool hasSword;
    public bool hasHook;
    public bool hasAmulet;
    public bool hasCombatArtifact;
    public bool hasPlatformArtifact;
    public bool hasPuzzleArtifact;

    // World
    public bool[] solvedPuzzles;
    public bool[] brokenWalls;

    public SaveData(PlayerData playerData/*, WorldData worldData */) {
        position[0] = playerData.transform.position.x;
        position[1] = playerData.transform.position.y;
        position[2] = playerData.transform.position.z;
        hasSword = playerData.hasSword;
        hasHook = playerData.hasHook;
        hasAmulet = playerData.hasAmulet;

        //hasCombatArtifact = ; TO ADD
        //hasPlatformArtifact = ;
        //hasPuzzleArtifact = ;

        // World saves must be done when unloading scenes. May be moved to a separate script.
    }
}
