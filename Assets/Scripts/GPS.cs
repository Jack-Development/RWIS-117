using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using TMPro;

public class GPS : MonoBehaviour
{
  public TextMeshProUGUI latitudeText;
  public TextMeshProUGUI longitudeText;

  private void Start()
  {
    #if PLATFORM_ANDROID
    if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
    {
        Permission.RequestUserPermission(Permission.FineLocation);
    }
    #endif
    StartCoroutine(StartLocationService());
  }

  private IEnumerator StartLocationService()
  {
    #if UNITY_EDITOR
    //Wait until Unity connects to the Unity Remote, while not connected, yield return null
    while (!UnityEditor.EditorApplication.isRemoteConnected)
    {
        yield return null;
    }
    #endif

    if (!Input.location.isEnabledByUser)
    {
      Debug.Log("Location services are not enabled by the user.");
      yield break;
    }

    Debug.Log("Starting location service");
    Input.location.Start();

    int maxWait = 20;
    while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
    {
      yield return new WaitForSeconds(1);
      maxWait--;
    }

    if (maxWait <= 0)
    {
      Debug.Log("Timed out");
      yield break;
    }

    if (Input.location.status == LocationServiceStatus.Failed)
    {
      Debug.Log("Unable to determine device location");
      yield break;
    }
    else if (Input.location.status == LocationServiceStatus.Running)
    {
      latitudeText.text = "Latitude: " + Input.location.lastData.latitude.ToString("F6");
      longitudeText.text = "Longitude: " + Input.location.lastData.longitude.ToString("F6");
    }
    else
    {
        Debug.Log("Location service is not running. Current status: " + Input.location.status);
    }

    Input.location.Stop();
  }
}
