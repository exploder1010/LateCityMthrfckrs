using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVehicle {

    void inputHorz(float direction); //handles rightwards and leftward input. 1 to -1.

    void inputAccel(float direction); //handles accelleration. 1 to -1.

    //void inputJump(int input); //handles jump input. 0 = none, 1 = button down, 2 = button stay, 3 = button up.

}
