using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Luminosity.IO
{
    public class LoadImageIfControls : MonoBehaviour
    {

        public GameObject keyboardImage;
        public GameObject gamepadImage;

        // Use this for initialization
        void Start()
        {
            if(InputManager.PlayerOneControlScheme.Name == "KeyboardAndMouse")
            {
                keyboardImage.SetActive(true);
                gamepadImage.SetActive(false);
            }

            else if (InputManager.PlayerOneControlScheme.Name == "Gamepad")
            {
                keyboardImage.SetActive(false);
                gamepadImage.SetActive(true);
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}

