using UnityEditor;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static CameraMovement Instance { get; private set; }
    [SerializeField] private float rotSmoothness;
    [SerializeField] private float moveSmoothness;
    [SerializeField] private Vector3 rotOffset;
    [SerializeField] private Vector3 moveOffset;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 observeOffset;
    private bool isMenu;
    public void HandleMovement()
    {
        if (target == null) return;
        Vector3 worldPosition = target.TransformPoint(moveOffset);
        transform.position = Vector3.Lerp(transform.position, worldPosition, moveSmoothness * Time.deltaTime);
    }

    public void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }

    public void HandleRotation()
    {
        if (target == null) return;
        //Vector3 distance =   target.position-transform.position;
        //Quaternion rot = new Quaternion();

        //rot = Quaternion.LookRotation(distance + rotOffset, Vector3.up);
        //transform.rotation = Quaternion.Lerp(transform.rotation,rot,rotSmoothness*Time.deltaTime);
        Quaternion lookRot = Quaternion.LookRotation(target.position - transform.position);

        Quaternion offsetRot = Quaternion.Euler(rotOffset);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            lookRot * offsetRot,
            rotSmoothness * Time.deltaTime);
    }
    void FixedUpdate()
    {
        if (isMenu)
        {
            HandleMenuCamera();
        }
        else
        {
            HandleMovement();
            HandleRotation();
        }
    }

    public void SetTarget(Transform target)
    {
        isMenu = false;
        this.target = target;

        //transform.position = target.TransformPoint(moveOffset);

        //Vector3 dir = target.position - transform.position;
        //transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    public void GetMenuPosition()
    {
        isMenu = true;
        
    }

    public void HandleMenuCamera()
    {
        CarController car = GameManager.Instance.GetCar();
        Quaternion lookRot = Quaternion.LookRotation(car.transform.position + observeOffset - transform.position);

        Quaternion offsetRot = Quaternion.Euler(rotOffset);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            lookRot * offsetRot,
            rotSmoothness * Time.deltaTime);
        transform.position = Vector3.Lerp(
            transform.position,
            car.cameraSpot.position,
            moveSmoothness * Time.deltaTime);
    }
}
