using UnityEngine;

public class CODE_RotatePlayer : MonoBehaviour
{
    public GameObject player;
    private Transform _playerTransform;
    private Vector3 _pos1, _pos2;
    
    public float sensitivity;
    private bool _holding;

    public bool isMoving;
    private void Start()
    {
        _playerTransform = player.GetComponent<Transform>();
    }

    private void Update()
    {
        //Clamp player position
        var position = _playerTransform.position;
        position = new Vector3(Mathf.Clamp(_playerTransform.transform.position.x, -8.5f, 8.5f), position.y, position.z);
        _playerTransform.position = position;

        //Swerve mechanic with rotate
        if (Input.GetMouseButtonDown(0))
        {
            _pos1 =  GameManager.Instance.GetMousePosition();
            _holding = true;
        }

        if (Input.GetMouseButton(0) && _holding) //set players velocity on X and Z axis
        {
            
            _pos2 = GameManager.Instance.GetMousePosition();

            var delta = _pos1 - _pos2;
            _pos1 = _pos2;
            
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                var localEulerAngles = player.transform.localEulerAngles;
                localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y + -delta.x * sensitivity * Time.deltaTime,player.transform.transform.localEulerAngles.z);
                localEulerAngles = new Vector3(GameManager.ClampAngle(localEulerAngles.x, -20, 20), GameManager.ClampAngle(localEulerAngles.y, -30, 30), localEulerAngles.z);
                player.transform.localEulerAngles = localEulerAngles;
                
            } 
            else
            {
                _playerTransform.rotation = Quaternion.Slerp(_playerTransform.rotation, Quaternion.Euler(0,0,0), Time.deltaTime * 3f);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _holding = false;
        }
    }
}
