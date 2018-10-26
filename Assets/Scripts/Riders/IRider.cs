using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRider {

    void inputHorz(float direction); //handles rightwards and leftward input. 1 to -1.

    void inputVert(float direction);//handles forwards and backwards input. 1 to -1.

    void inputAbility(int input); //handles jump input. 0 = none, 1 = button down, 2 = button stay, 3 = button up.

    void inputBreakIn(int input); //handles jump input. 0 = none, 1 = button down, 2 = button stay, 3 = button up.
}
