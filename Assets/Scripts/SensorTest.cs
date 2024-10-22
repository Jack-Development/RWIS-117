using UnityEngine;

public class SensorTest : MonoBehaviour
{
    private Gyroscope gyro;
    private bool gyroEnabled;
    private Quaternion adjustQuaternion = Quaternion.Euler(90f, 0f, 0f);
    public GameObject phone;


    void Start()
    {
      Debug.Log("Gyro Enabled: " + SystemInfo.supportsGyroscope);
      gyroEnabled = EnableGyro();
    }

    private bool EnableGyro()
    {
      if (SystemInfo.supportsGyroscope)
      {
        gyro = Input.gyro;
        gyro.enabled = true;
        return true;
      }
      return false;
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
      return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    void Update()
    {
      if (gyroEnabled)
      {
        Debug.Log("Gyro Attitude: " + gyro.attitude);
        Debug.Log("Gyro Gravity: " + gyro.gravity);
        Debug.Log("Gyro RotationRate: " + gyro.rotationRate);
        Debug.Log("Gyro UserAcceleration: " + gyro.userAcceleration);

        Quaternion gyroQuaternion = GyroToUnity(gyro.attitude);
        Quaternion correctRotation = adjustQuaternion * gyroQuaternion;
        phone.transform.rotation = correctRotation;
      }
    }
}
