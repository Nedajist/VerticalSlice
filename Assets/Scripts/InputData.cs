using UnityEngine;
using System.Linq;

public class InputData : ScriptableObject
{
    public string inputType;
    public int inputFrame; // the frame at which the input began to be pressed
    public int heldFrames = 1; // number of frames the input was held. Its lifespan.
    public Vector2 movementData; // vector2 on what direction character will move 
    public Vector3 mousePosition;
    public float abilityData; // selecting 1-2
    public bool startedExecution = false; // whether or not this object has previously been executed 
    
    public void SetValues(InputData old_input_data)
    {
        inputType = old_input_data.inputType;
        inputFrame = old_input_data.inputFrame;
        heldFrames = old_input_data.heldFrames;
        movementData = old_input_data.movementData;
        mousePosition = old_input_data.mousePosition;
        abilityData = old_input_data.abilityData;
        startedExecution = false;
    }


}
