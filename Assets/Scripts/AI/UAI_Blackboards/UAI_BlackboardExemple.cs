using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UAI_BlackboardExemple", menuName = "ScriptableObjects/Blackboard/BlackboardExemple", order = 1)]
public class UAI_BlackboardExemple : UAI_Blackboard
{
    public int MyValue = 0;
    public string MyString = "Salut Noé !";
}
