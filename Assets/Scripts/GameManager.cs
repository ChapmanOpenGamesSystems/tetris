using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Tooltip("Determines if player can keep playing or not")]
    public bool death;

    public static GameManager Instance;

    [Tooltip("a float representing player input for movement, If negative move left, if positive move right")]
    public float horizontalMovement;

    public bool rotate;
    public bool PlayerTurn;

    public Color PlayerControlledColor;
    public Color StableColor;
    public Color BackgroundColor;

    public float prevHorizontalMovement;

    [SerializeField]
    public List<GameObject> prefabShapesToGenerate;

    public List<Color> ShapeColors;




    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance == null)
        {
            GameManager.Instance = this;
        }
        else
        {
            Destroy(this);
        }

        if(prefabShapesToGenerate.Count == 0)
        {
            Debug.Log("There are no shapes to generate");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        prevHorizontalMovement = horizontalMovement;
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        rotate = Input.GetButtonDown("Jump");
        if(Input.GetButtonDown("Cancel"))
        {
            Quit();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public GameObject GetShapeToGenerate()
    {
        int numShapes = prefabShapesToGenerate.Count;
        float randValue = Random.value;
        float index = randValue;
        for (int i = 1; i <= numShapes; ++i)
        {
            if (index <= i / (float)numShapes)
            {
                index = i-1;
                break;
            }
        }

        int numColors = ShapeColors.Count;
        float colorIndex = randValue;
        for (int i = 1; i <= numColors; ++i)
        {
            if (colorIndex <= i / (float)numColors)
            {
                colorIndex = i - 1;
                break;
            }
        }

        PlayerControlledColor = ShapeColors[(int)colorIndex];
        return prefabShapesToGenerate[(int)index]; 
    }
}
