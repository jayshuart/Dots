using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Player", menuName = "Custom/Player")]
public class Player : ScriptableObject
{
    public string name;
    public Color color;
}
