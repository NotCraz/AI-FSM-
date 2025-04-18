using UnityEngine;

public class character : MonoBehaviour
{
    private CharacterController characterController;

    public float speed = 5f;
    void Start()
    {
        characterController = GetComponent<CharacterController>();  
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        characterController.Move(move * Time.deltaTime * speed);    
    }
}
