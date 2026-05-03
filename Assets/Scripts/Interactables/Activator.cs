using UnityEngine;

public enum ActivatorType { Star, Player, Either }

public class Activator : MonoBehaviour
{
    public ActivatorType type = ActivatorType.Either;
}
