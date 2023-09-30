using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HudController : MonoBehaviour
{
    static public float lightExposure = 0, fuel;
    static public bool dead;

    private Slider lightExposureSlider, burnLevelSlider, fuelLevelSlider;
    private Image lightExposureImage, burnLevelImage, fuelLevelImage;

    private GameObject hudStats;
    static GameObject deadPanel, winPanel;

    [SerializeField] private float burnSpeed;
    private float startDyingMoment;
    static  float finishMoment;

    void Start()
    {
        hudStats = transform.GetChild(0).gameObject;
        deadPanel = transform.Find("Death").gameObject;
        winPanel = transform.Find("Win").gameObject;

        deadPanel.SetActive(false);
        winPanel.SetActive(false);

        lightExposureSlider = hudStats.transform.Find("LightExposure").GetComponent<Slider>();
        burnLevelSlider = hudStats.transform.Find("BurnLevel").GetComponent<Slider>();
        fuelLevelSlider = hudStats.transform.Find("FuelLevel").GetComponent<Slider>();

        lightExposureImage = lightExposureSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        burnLevelImage = burnLevelSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
        fuelLevelImage = fuelLevelSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }

    private void LateUpdate()
    {
        ManageSliders();

        ShowSliders();

        lightExposure = 0;

        if (finishMoment != 0 && Time.time - finishMoment >= 3)
        {
            Application.Quit();
        }
    }

    void ManageSliders()
    {
        // Manage Light slider.

        lightExposureSlider.value = lightExposure;

        // Manage Burn slider.

        if (lightExposure != 0 && burnLevelSlider.value < 1)
        {
            //Make it increase ->

            float burnIncrease = lightExposure / burnSpeed * Time.deltaTime;

            burnLevelSlider.value += burnIncrease;

            if (burnLevelSlider.value >= 1) startDyingMoment = Time.time;

            //Make it decrease ->
        }
        else if (burnLevelSlider.value != 0 && lightExposure == 0) burnLevelSlider.value -= Time.deltaTime;

        if (Time.time - startDyingMoment >= 2 && burnLevelSlider.value >= 1) Die();

        // Manage Fuel slider.

        fuelLevelSlider.value = fuel;
    }

    private void ShowSliders()
    {
        // Making the sliders look completely empty.

        if (lightExposure == 0) lightExposureImage.enabled = false;
        else lightExposureImage.enabled = true;

        if (burnLevelSlider.value == 0) burnLevelImage.enabled = false;
        else burnLevelImage.enabled = true;

        if (fuel == 0) fuelLevelImage.enabled = false;
        else fuelLevelImage.enabled = true;
    }

    static public void Die()
    {
        if (dead) return;

        GameObject playerGO = GameObject.Find("Player");

        Destroy(playerGO);
        dead = true;

        finishMoment = Time.time;

        deadPanel.SetActive(true);
    }

    static public void Win()
    {
        dead = true;
        winPanel.SetActive(true);
    }
}