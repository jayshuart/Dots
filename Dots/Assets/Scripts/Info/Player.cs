using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Player", menuName = "Custom/Player")]
public class Player : ScriptableObject
{
    public string playerName;
    public Color color;
}
