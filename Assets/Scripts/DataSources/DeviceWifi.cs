﻿using System;
using System.Collections;
using System.Collections.Generic;
using Imported.FSG.AndroidWifiManager.Scripts;
using UnityEngine;

namespace DataSources
{
    public class DeviceWifi : MonoBehaviour
    {
        [SerializeField] private float scanIntervalInSeconds = 35;
        private static DeviceWifi _instance;

        private static List<AndroidWifiScanResults> _wifiData = new List<AndroidWifiScanResults>();

        private void Awake()
        {
            _instance = this;
        }

        public static void Enable()
        {
            _instance.StartCoroutine(InitialisationCoroutine());
        }

        public static WifiData GetWifiData()
        {
            return new WifiData()
            {
                wifiScanResults = _wifiData
            };
        }

        private static IEnumerator InitialisationCoroutine()
        {
            if (!AndroidWifiManager.IsWifiEnabled())
            {
                Debug.Log("Wifi is disabled. Starting up");
                AndroidWifiManager.SetWifiEnabled(true);
                yield return new WaitForSecondsRealtime(1);
            }

            Debug.Log("Wifi is enabled. Starting scanning coroutine...");
            _instance.StartCoroutine(UpdateWifiDataCoroutine());
        }

        private static IEnumerator UpdateWifiDataCoroutine()
        {
            while (true)
            {
                var scanStarted = AndroidWifiManager.StartScan();

                if (!scanStarted)
                    Debug.LogError("Wifi scan failed to start");

                yield return new WaitForSecondsRealtime(1);

                _wifiData = AndroidWifiManager.GetScanResults();

                yield return new WaitForSecondsRealtime(_instance.scanIntervalInSeconds);
            }
        }

        [Serializable]
        public class WifiData
        {
            public List<AndroidWifiScanResults> wifiScanResults;
        }
    }
}