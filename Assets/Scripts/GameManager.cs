using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    int currentTest = 0;

    public TextMeshProUGUI TutorialText;
    public PlayerMovement PlayerMovement;
    GenericMeshGravity[] _gravitySources;
    Vector3 playerStartingPos;
    public GravitySource startingGravSource;


    void Start()
    {
        _gravitySources = FindObjectsOfType<GenericMeshGravity>();
        playerStartingPos = PlayerMovement.transform.position;
    }

    private void Update()
    {
        string textString = "Press R to Restart\nPress N to move to the next Test\nCurrent Test: ";
        float baseGravityStrength = -28.89f;
        float gravityStrength = baseGravityStrength;
        float jumpHeight = 2.19f;
        float playerSpeed = 8.5f;
        float playerAcceleration = 33.01f;

        if (currentTest == 0)
        {
            //Default values
            gravityStrength *= 0.5f;

            textString += "A1\n";
            textString += "Try jumping around and see how the gravity feels";
        }

        if (currentTest == 1)
        {
            textString += "A2\n";
            gravityStrength *= 1.5f;
            textString += "Try jumping around and see how the gravity feels";
        }

        if (currentTest == 2)
        {
            textString += "A3\n";
            gravityStrength *= 1f;
            textString += "Try jumping around and see how the gravity feels";
        }

        if (currentTest == 3)
        {
            textString += "A4\n";
            gravityStrength *= 2f;
            textString += "Try jumping around and see how the gravity feels";
        }

        if (currentTest == 4)
        {
            textString += "B1\n";
            PlayerMovement._jumpHeight = jumpHeight * 0.5f;
            textString += "Try jumping around and see how the gravity feels";
        }

        if (currentTest == 5)
        {
            textString += "B2\n";
            PlayerMovement._jumpHeight = jumpHeight * 1.5f;
            textString += "Try jumping around and see how the gravity feels";
        }
        if (currentTest == 6)
        {
            textString += "B3\n";
            PlayerMovement._jumpHeight = jumpHeight * 1f;
            textString += "Try jumping around and see how the gravity feels";
        }
        if (currentTest == 7)
        {
            textString += "B4\n";
            PlayerMovement._jumpHeight = jumpHeight * 2f;
            textString += "Try jumping around and see how the gravity feels";
        }
        if (currentTest == 8)
        {
            textString += "C1\n";
            PlayerMovement._maxSpeed = playerSpeed * 0.5f;
            textString += "Try moving around and see how the movement feels";
        }
        if (currentTest == 9)
        {
            textString += "C2\n";
            PlayerMovement._maxSpeed = playerSpeed * 1.5f;
            textString += "Try moving around and see how the movement feels";
        }
        if (currentTest == 10)
        {
            textString += "C3\n";
            PlayerMovement._maxSpeed = playerSpeed * 1f;
            textString += "Try moving around and see how the movement feels";
        }
        if (currentTest == 11)
        {
            textString += "C4\n";
            PlayerMovement._maxSpeed = playerSpeed * 2f;
            textString += "Try moving around and see how the movement feels";
        }

        if (currentTest == 12)
        {
            textString += "D1\n";
            PlayerMovement._maxAcceleration = playerAcceleration * 0.5f;
            textString += "Try moving around and see how the movement feels";
        }

        if(currentTest == 13)
        {
            textString += "D2\n";
            PlayerMovement._maxAcceleration = playerAcceleration * 1.5f;
            textString += "Try moving around and see how the movement feels";
        }
        if (currentTest == 14)
        {
            textString += "D3\n";
            PlayerMovement._maxAcceleration = playerAcceleration * 1f;
            textString += "Try moving around and see how the movement feels";
        }
        if (currentTest == 15)
        {
            textString += "D4\n";
            PlayerMovement._maxAcceleration = playerAcceleration * 2f;
            textString += "Try moving around and see how the movement feels";
        }

        foreach (GenericMeshGravity gravitySource in _gravitySources)
        {
            gravitySource.gravityStrength = gravityStrength;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            currentTest++;
            if (currentTest > 15)
            {
                currentTest = 0;
            }
            PlayerMovement.transform.position = playerStartingPos;
            CustomGravity.Instance.SetGravitySource(startingGravSource);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerMovement.transform.position = playerStartingPos;
            CustomGravity.Instance.SetGravitySource(startingGravSource);
        }

        TutorialText.text = textString;
    }
}
