using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask obstaclesLayer;
    [SerializeField] LayerMask interactablesLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask fovLayer;
    public static GameLayers i { get; set; }
    private void Awake()
    {
        i=this;
    }
    public LayerMask ObstaclesLayer { get => obstaclesLayer; }
    public LayerMask InteractablesLayer { get => interactablesLayer; }
    public LayerMask PlayerLayer { get => playerLayer; }
    public LayerMask FOVLayer { get => fovLayer; }
}
