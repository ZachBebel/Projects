using UnityEngine;
using System.Collections;

public class AssetManager : MonoBehaviour
{
    // Materials

    // "Resources.Load" is able to access and assign assets without the inspector
    // Assets to be loaded must be located in any "Resources" folder within the "Assets" folder

    // Player Colors
    public static Material redMat = (Material)Resources.Load("Red", typeof(Material));
    public static Material greenMat = (Material)Resources.Load("Green", typeof(Material));
    public static Material goldMat = (Material)Resources.Load("Gold", typeof(Material));
    public static Material blueMat = (Material)Resources.Load("Blue", typeof(Material));

    public static Material[] colors = { redMat, greenMat, goldMat, blueMat };

    // Health Bar Components
    public static GameObject healthBar = (GameObject)Resources.Load("UI Health Bar", typeof(GameObject));
    public static Texture greenHealthMat = (Texture)Resources.Load("healthBarSlantGreen", typeof(Texture));
    public static Texture redHealthMat = (Texture)Resources.Load("healthBarSlantRed", typeof(Texture));

    // HUD Components
    public static GameObject HUDObj = (GameObject)Resources.Load("HUD", typeof(GameObject));
    public static Texture HUDDefault = (Texture)Resources.Load("hud", typeof(Texture));
    public static Texture HUDRed = (Texture)Resources.Load("hudRed", typeof(Texture));
    public static Texture HUDGreen = (Texture)Resources.Load("hudGreen", typeof(Texture));
    public static Texture HUDGold = (Texture)Resources.Load("hudYellow", typeof(Texture));
    public static Texture HUDBlue = (Texture)Resources.Load("hudBlue", typeof(Texture));

    public static Texture[] HUDColors = { HUDRed, HUDGreen, HUDGold, HUDBlue };

    // Pause/Instruction Menu Image
    public static Texture instructionsTexture = (Texture)Resources.Load("Instructions", typeof(Texture));

    // Title Image
    public static Texture titleTexture = (Texture)Resources.Load("JumpTheGun", typeof(Texture));

    // Levels
    public static GameObject levelFPSTeleport = (GameObject)Resources.Load("[LEVEL] FPSTeleport", typeof(GameObject));
    public static GameObject levelTheConfines = (GameObject)Resources.Load("[LEVEL] The Confines", typeof(GameObject));
    public static GameObject levelTheDomain = (GameObject)Resources.Load("[LEVEL] The Domain", typeof(GameObject));
    public static GameObject levelDrone = (GameObject)Resources.Load("[LEVEL] Drone", typeof(GameObject));

    public static GameObject[] levels = { /*levelFPSTeleport,
                                            levelTheConfines,
                                            levelTheDomain, */
                                            levelDrone };
}
