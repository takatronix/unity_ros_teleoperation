using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ROSManager : MonoBehaviour
{
    public delegate void ConnectionColor(Color color);
    public ConnectionColor OnConnectionColor;

    private List<string> _ips;
    public string defaultIP = "10.42.0.1";
    public string defaultPort = "10000";

    public GameObject ipSetting;
    public GameObject portSetting;
    private string _ip;
    private int _port;

    private TMPro.TMP_InputField _ipText;
    private TMPro.TextMeshProUGUI _portText;

    private ROSConnection _ros;
    private bool _connected = false;

    void Start()
    {
        _ips = new List<string>();
        string ips = "";
        if (PlayerPrefs.HasKey("ips"))
        {
            ips = PlayerPrefs.GetString("ips");
        }
        foreach(string ip in ips.Split(',')) {
            _ips.Add(ip);
        }

        if(PlayerPrefs.HasKey("ip"))
        {
            _ip = PlayerPrefs.GetString("ip");
        }
        else
        {
            _ip = defaultIP;
        }

        if(PlayerPrefs.HasKey("port"))
        {
            _port = PlayerPrefs.GetInt("port");
        }
        else
        {
            _port = int.Parse(defaultPort);
        }


        _ipText = ipSetting.GetComponent<TMPro.TMP_InputField>();
        _portText = portSetting.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        _ros = ROSConnection.GetOrCreateInstance();


        _ros.RosPort = _port;
        _ros.RosIPAddress = _ip;

        _ros.Connect();

        _ipText.text = _ip;
        _portText.text = _port.ToString();

    }

    public void OnIPDone(string ip)
    {
        _ip = ip;
        _ros.RosIPAddress = _ip;
        PlayerPrefs.SetString("ip", _ip);
        PlayerPrefs.Save();

    }

    void Update()
    {
        if(_connected != _ros.IsConnected)
        {
            _connected = _ros.IsConnected;
            if(_connected)
            {
                OnConnectionColor?.Invoke(Color.green);
            }
            else
            {
                OnConnectionColor?.Invoke(Color.red);
            }
        }

    }

    public void SaveIP()
    {
        if(!_ips.Contains(_ip))
        {
            _ips.Insert(0, _ip);
        }
        PlayerPrefs.SetString("ips", string.Join(",", _ips));
        PlayerPrefs.Save();
    }
}
