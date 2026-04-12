using UnityEngine;

[CreateAssetMenu(fileName = "NewDaddyProfile", menuName = "Game/Daddy Profile")]
public class DaddyProfile : ScriptableObject
{
    public string daddyName;

    [TextArea(3, 10)]
    public string description;
}
