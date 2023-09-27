using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using TMPro;
using UnityEngine.UI;
using System;
using static System.Net.Mime.MediaTypeNames;


enum TaskState
{
    INIT,
    WAIT_COMMANDS
}

public class Test : MonoBehaviour
{
    public Slider temperaturaSlider;
    public Slider altitudSlider;
    public Slider presionSlider;
    public Slider velocidadSlider;

    public TextMeshProUGUI temperaturaText;
    public TextMeshProUGUI alturaText;
    public TextMeshProUGUI presionText;
    public TextMeshProUGUI velocidadText;

    public TextMeshProUGUI acertijosResueltosText;
    public TextMeshProUGUI acertijosTotalesText;
    public TextMeshProUGUI nivelDificultadText;
    public TextMeshProUGUI barraTemperatura;



    public Camera camaraEscena;
    public GameObject menuConfig;
    public GameObject hud;

    private int varTemp;
    private int varPressure;
    private int varHeight;
    private int varSpeed;
    private int varSolvedPuzzles = 0;
    private int varDifficulty = 0;


    private int tempInicial;
    private float timerPuzzle;
    private float timerDifficulty;
    private float tiempoEntreAumentos = 10.0f;
    private float tiempoDificultad = 30.0f;

    public Button boton;

    public bool configurado = false;

    private static TaskState taskState = TaskState.INIT;
    private SerialPort _serialPort;
    private byte[] buffer;
    public TextMeshProUGUI myText;
    //private int counter = 0;

    void Start()
    {
        boton.onClick.AddListener(cambiarPosicionCamara);
        //hud.SetActive(false);

        temperaturaSlider.value = 25; // Valor inicial de la temperatura.
        altitudSlider.value = 500;    // Valor inicial de la altitud.
        presionSlider.value = 13;   // Valor inicial de la presión atmosférica.
        velocidadSlider.value = 1;
        timerPuzzle = 0.0f;
        timerDifficulty = 0.0f;


        _serialPort = new SerialPort();
        _serialPort.PortName = "COM4";
        _serialPort.BaudRate = 115200;
        _serialPort.DtrEnable = true;
        _serialPort.NewLine = "\n";
        _serialPort.Open();
        //Debug.Log("Open Serial Port");
        buffer = new byte[128];
    }

    void Update()
    {
        varTemp = Convert.ToInt32(temperaturaSlider.value);
        //varPressure = Convert.ToInt32(presionSlider.value);
        //varHeight = Convert.ToInt32(altitudSlider.value);
        //varSpeed = Convert.ToInt32(velocidadSlider.value);

        //alturaText.text = "Altura: " + varHeight;
        //presionText.text = "Presión: " + varPressure;
        temperaturaText.text = "Temperatura: " + varTemp;
        acertijosResueltosText.text = "Acertijos resueltos: " + varSolvedPuzzles;
        nivelDificultadText.text = "Nivel de dificultad: " + varDifficulty;
        //velocidadText.text = "Velocidad: " + varSpeed;

        switch (taskState)
        {
            case TaskState.INIT:
                taskState = TaskState.WAIT_COMMANDS;
                Debug.Log("WAIT COMMANDS");
                break;
            case TaskState.WAIT_COMMANDS:

                timerPuzzle += Time.deltaTime;
                if (timerPuzzle >= tiempoEntreAumentos)
                {
                    varSolvedPuzzles++;
                    timerPuzzle = 0.0f;
                    Debug.Log("Acertijos resueltos: " + varSolvedPuzzles);
                }

                timerDifficulty += Time.deltaTime;
                if (timerDifficulty >= tiempoDificultad)
                {
                    varDifficulty++;
                    timerDifficulty = 0.0f;
                    Debug.Log("Nivel de dificultad: " + varDifficulty);
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    tempInicial = varTemp;
                    _serialPort.Write("ledON\n");
                    _serialPort.Write(varTemp.ToString());
                    Debug.Log("Send ledON");
                    //cambiarPosicionCamara();
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    _serialPort.Write("ledOFF\n");
                    Debug.Log("Send ledOFF");
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    _serialPort.Write("readBUTTONS\n");
                    Debug.Log("Send readBUTTONS");
                }
                if (_serialPort.BytesToRead > 0)
                {
                    string response = _serialPort.ReadLine();
                    Debug.Log(response);

                    // Separar los valores del mensaje
                    string[] values = response.Split(new char[] { ':', ' ' });

                    // Asignar los valores a las variables
                    varTemp = Convert.ToInt32(values[1]);
                    varHeight = Convert.ToInt32(values[3]);
                    varPressure = Convert.ToInt32(values[5]);

                    // Actualizar las etiquetas
                    alturaText.text = "Altura: " + varHeight;
                    presionText.text = "Presión: " + varPressure;
                    temperaturaText.text = "Temperatura: " + varTemp;


                    //varTemp = Convert.ToInt32(response);
                    /*int numAcertijos = tempInicial;

                    acertijosTotalesText.text = "Acertijos Totales: " + numAcertijos;
                    barraTemperatura.text = "Temp actual: " + varTemp;*/
                }
                break;

            default:
                Debug.Log("State Error");
                break;
        }
    }

    void cambiarPosicionCamara()
    {
        camaraEscena.transform.position = new Vector3(76, 3, -28);
        menuConfig.SetActive(false);
        hud.SetActive(true);

        configurado = true;

        return;
    }
}
