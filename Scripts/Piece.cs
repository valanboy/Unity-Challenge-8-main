using UnityEngine;

public enum PieceType
    {
    none = -1,
    ramp = 0,
    longblock = 1,
    jump = 2,
    slide = 3,
    }

public class Piece : MonoBehaviour
{
    public PieceType type;
    public int visualIndex;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
