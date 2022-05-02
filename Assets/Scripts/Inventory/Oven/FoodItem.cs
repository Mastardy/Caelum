using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "FoodItem", menuName = "ScriptableObjects/FoodItem", order = 4)]
public class FoodItem : ScriptableObject
{
    public int id;
    [FormerlySerializedAs("food")] public float hunger;
    public float thirst;
    public float temperature;
    public float poison;
}
