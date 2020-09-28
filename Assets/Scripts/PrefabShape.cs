using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabShape : MonoBehaviour
{

    /// <summary>
    /// This Class contains the base info of a shape to be generated
    /// The first coordinate should be the center piece and the rest 
    /// should be offsetv from it based on the coordinates they contain
    /// </summary>
    /// 
    public List<Coordinate> shapeInfo;

    public bool enableRotate;

    [Tooltip("A value between 0 and 3 inclusive, determines which rotation is used on the shape")]
    public int rotateState;
}
