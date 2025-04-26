using UnityEngine;

public class ConstantSpin : MonoBehaviour
{
    [SerializeField]
    float spinSpeed;

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(new Vector3(0, 1, 0), spinSpeed);   
    }
}
