using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputWord : MonoBehaviour
{
    public InputField playerInput;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void change()
    {
        Debug.Log(playerInput.text);
    }
}
