using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Composition 
{
    // Toutes les compositions possibles de groupes et sous groupes en fonction du nombre de héros
    public enum composition
    {
        Hero1_1 = 0,

        Hero2_2 = 1,
        Hero2_1_1 = 2,

        Hero3_3 = 3,
        Hero3_2_1 = 4,
        Hero3_1_1_1 = 5,

        Hero4_4 = 6,
        Hero4_3_1 = 7,
        Hero4_2_2 = 8,
        Hero4_2_1_1 = 9,
        Hero4_1_1_1_1 = 10
    }
}
