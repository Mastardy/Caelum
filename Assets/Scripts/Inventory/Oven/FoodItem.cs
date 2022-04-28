using UnityEngine;

[CreateAssetMenu(fileName = "FoodItem", menuName = "ScriptableObjects/FoodItem", order = 4)]
public class FoodItem : ScriptableObject
{
    public int id;
    public float food;
    public float thirst;
    public float temperature;
    public float poison;
}
