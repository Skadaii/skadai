using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public virtual void MoveToward(Vector3 velocity) { }
    public virtual void MoveTo(Vector3 position) { }

}
