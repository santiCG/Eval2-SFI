using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using System;

public class Serial : MonoBehaviour
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


    public Camera camaraEscena;
    public GameObject menuConfig;
    public GameObject hud;

    public Button boton;

    public bool configurado = false;


    private SerialPort _serialPort = new SerialPort();
    //private byte[] buffer = new byte[32];

    private int varTemp;
    private int varPressure;
    private int varHeight;
    private int varSpeed;


    public void Start()
    {
        boton.onClick.AddListener(cambiarPosicionCamara);
        hud.SetActive(false);

        _serialPort.PortName = "COM4";
        _serialPort.BaudRate = 115200;
        _serialPort.DtrEnable = true;
        _serialPort.Open();
        Debug.Log("Open Serial Port");

        temperaturaSlider.value = 25; // Valor inicial de la temperatura.
        altitudSlider.value = 500;    // Valor inicial de la altitud.
        presionSlider.value = 13;   // Valor inicial de la presión atmosférica.
        velocidadSlider.value = 1;
    }

    void Update()
    {
        varTemp = Convert.ToInt32(temperaturaSlider.value);
        varPressure = Convert.ToInt32(presionSlider.value);
        varHeight = Convert.ToInt32(altitudSlider.value);
        varSpeed = Convert.ToInt32(velocidadSlider.value);

        Debug.Log(varTemp);

        alturaText.text = "Altura: " + varHeight;
        presionText.text = "Presión: " + varPressure;
        temperaturaText.text = "Temperatura: " + varTemp;
        acertijosTotalesText.text = "Acertijos Totales: " + varTemp;
        velocidadText.text = "Velocidad: " + varSpeed;

        if (_serialPort.BytesToRead > 0) // Verificar si hay datos disponibles para leer
        {
            string message = _serialPort.ReadLine(); // Leer el mensaje
            if (message[0] == 'T') // Verificar si el mensaje es acerca de la temperatura
            {
                float newTemperature = float.Parse(message.Substring(1)); // Convertir el resto del mensaje a un número
                temperaturaSlider.value = newTemperature; // Actualizar el valor del slider
                acertijosTotalesText.text = "Temperatura: " + newTemperature.ToString(); // Actualizar el texto de la temperatura
            }
        }

        if (configurado == true)
        {
            //string message = "T" + varTemp; // Crear un mensaje con el valor del slider
            //_serialPort.WriteLine(message); // Enviar el mensaje al microcontrolador

            //Crea una cadena de datos con los valores
            string dataToSend = varHeight.ToString("F2") + "," + varPressure.ToString("F2") + "," + varTemp.ToString("F2") + varSpeed.ToString("F2") + "\n";

            // Envía la cadena de datos al Raspberry Pi Pico a través de la comunicación serial
            if (_serialPort.IsOpen)
            {
                _serialPort.Write(dataToSend);
            }

            if (_serialPort.BytesToRead > 0)
            {
                // Lee la respuesta y guárdala en una variable (en este ejemplo, simplemente mostramos la respuesta)
                string response = _serialPort.ReadLine();
                Debug.Log("Respuesta del Raspberry Pi Pico: " + response);
            }


        }
    }

    void cambiarPosicionCamara()
    {
        camaraEscena.transform.position = new Vector3(76, 3, -28);
        menuConfig.SetActive(false);
        hud.SetActive(true);

        configurado = true;
    }
}