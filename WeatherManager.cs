using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.Android;

public class WeatherManager : MonoBehaviour
{


    [Header("UI")]
    public TextMeshProUGUI statusText;

    public TextMeshProUGUI location;
    public TextMeshProUGUI country;

    public TextMeshProUGUI mainWeather;
    public TextMeshProUGUI description;
    public TextMeshProUGUI temp;
    public TextMeshProUGUI feels_like;
    public TextMeshProUGUI temp_min;
    public TextMeshProUGUI temp_max;
    public TextMeshProUGUI pressure;
    public TextMeshProUGUI humidity;
    public TextMeshProUGUI windspeed;
    private LocationInfo lastLocation;

  //  public TextMeshProUGUI text;
    void Start() {
        StartCoroutine(FetchLocationData());
    }
    private IEnumerator FetchLocationData() {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        // First, check if user has location service enabled 
        if (!Input.location.isEnabledByUser) yield break; 
        // Start service before querying location 
        Input.location.Start(); 
        // Wait until service initializes 
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        } 
        // Service didn't initialize in 20 seconds 
        if (maxWait < 1) { statusText.text = "Location Timed out";
            yield break;
        }
        // Connection has failed 
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            statusText.text = "Unable to determine device location";
            yield break;
        } else {
            lastLocation = Input.location.lastData;
            // text.text = lastLocation.latitude.ToString() + "   " + lastLocation.longitude.ToString();
            UpdateWeatherData();
        }
       //Input.location.Stop();
    }
    private void UpdateWeatherData() {
        StartCoroutine(FetchWeatherDataFromApi(lastLocation.latitude.ToString(), lastLocation.longitude.ToString()));
    }
    private IEnumerator FetchWeatherDataFromApi(string latitude, string longitude)
    {
        string url = "http://api.openweathermap.org/data/2.5/weather?lat=" + latitude + "&lon=" + longitude + "&appid=f3203502726ee6954c946755935f141d&units=metric";
        UnityWebRequest fetchWeatherRequest = UnityWebRequest.Get(url);
        yield return fetchWeatherRequest.SendWebRequest();
            if (fetchWeatherRequest.result == UnityWebRequest.Result.ConnectionError || fetchWeatherRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            //Check and print error
            statusText.text = fetchWeatherRequest.error;
        }
        else
        {
            Debug.Log(fetchWeatherRequest.downloadHandler.text);
            var response = JSON.Parse(fetchWeatherRequest.downloadHandler.text);
            
            // country.text = response["country"];

            // mainWeather.text = response["weather"][0]["main"];
            description.text = response["weather"][0]["description"];
            location.text = response["name"];
            temp.text = response["main"]["temp"] + " C";
            feels_like.text = "Feels like " + response["main"]["feels_like"] + " C";
            temp_min.text = "Min Temp - " + response["main"]["temp_min"] + " C";
            temp_max.text = "Max Temp - " + response["main"]["temp_max"] + " C";
            pressure.text = "Pressure - " + response["main"]["pressure"] + " mbar";
            humidity.text = "Humidity - " + response["main"]["humidity"] + " %";
            windspeed.text = "Windspeed - " + response["wind"]["speed"] + " Km/h";

        }
    }
}