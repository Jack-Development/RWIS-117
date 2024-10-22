using System.Collections;
using UnityEngine;
using UnityEngine.Android;
using TMPro;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Core.Coordinates;

public class PlayerLocation : MonoBehaviour
{
    public TextMeshProUGUI latitudeText;
    public TextMeshProUGUI longitudeText;
    public TextMeshProUGUI altitudeText;
    public LightshipMapView mapView;
    public GameObject markerPrefab;
    public GameObject originPrefab;

    private GameObject marker;
    private GameObject originMarker;
    private double latitude;
    private double longitude;
    private double altitude;
    private bool locationStarted = false;

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

    private void LateUpdate(){
        if(Input.location.status == LocationServiceStatus.Running && locationStarted){
            latitude = (double)Input.location.lastData.latitude;
            longitude = (double)Input.location.lastData.longitude;
            altitude = (double)Input.location.lastData.altitude;

            latitudeText.text = "Latitude: " + latitude.ToString("F6");
            longitudeText.text = "Longitude: " + longitude.ToString("F6");
            altitudeText.text = "Altitude: " + altitude.ToString("F6");
            UpdateMarker(latitude, longitude);
        }
    }

    private void OnDestroy()
    {
        Input.location.Stop();
    }

    private IEnumerator StartLocationService()
    {
        bool isUnityRemote = false;
        #if UNITY_EDITOR
        isUnityRemote = true;
        #endif

        if (isUnityRemote)
        {
            yield return new WaitForSeconds(2);
        }

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services are not enabled by the user.");
            yield break;
        }

        Debug.Log("Starting location service");
        Input.location.Start(1, 0.5f);


        if (isUnityRemote)
        {
            yield return new WaitForSeconds(2);
        }

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
            latitude = (double)Input.location.lastData.latitude;
            longitude = (double)Input.location.lastData.longitude;
            altitude = (double)Input.location.lastData.altitude;

            latitudeText.text = "Latitude: " + latitude.ToString("F6");
            longitudeText.text = "Longitude: " + longitude.ToString("F6");
            altitudeText.text = "Altitude: " + altitude.ToString("F6");

            mapView.SetMapCenter(new LatLng(latitude, longitude));
            UpdateMarker(latitude, longitude);
            locationStarted = true;
        }
        else
        {
            Debug.Log("Location service is not running. Current status: " + Input.location.status);
        }
    }

    private void UpdateMarker(double latitude, double longitude)
    {
        LatLng position = new LatLng(latitude, longitude);
        Vector3 worldPos;
        if (marker == null)
        {
            marker = Instantiate(markerPrefab);
            originMarker = Instantiate(originPrefab);

            worldPos = mapView.LatLngToScene(position);
            worldPos.y = 450;
            originMarker.transform.position = worldPos;
            originMarker.transform.SetParent(mapView.gameObject.transform);
        }
        worldPos = mapView.LatLngToScene(position);
        worldPos.y = 500;
        marker.transform.position = worldPos;
    }
}
