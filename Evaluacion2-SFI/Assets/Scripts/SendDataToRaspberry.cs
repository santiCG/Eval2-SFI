using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

public class SendDataToRaspberry : MonoBehaviour
{
    public Slider altitudeSlider;
    public Slider pressureSlider;
    public Slider temperatureSlider;
    public Slider velocitySlider;

    private SerialPort serialPort;
    private string portName = "COM4"; // Cambia esto al puerto correcto

    private void Start()
    {
        // Configura la comunicación serial
        serialPort = new SerialPort(portName, 9600);
        serialPort.Open();
    }

    private void Update()
    {
        // Obtiene los valores de los sliders
        float altitude = altitudeSlider.value;
        float pressure = pressureSlider.value;
        float temperature = temperatureSlider.value;
        float vel = velocitySlider.value;

        // Crea una cadena de datos con los valores
        string dataToSend = altitude.ToString("F2") + "," + pressure.ToString("F2") + "," + temperature.ToString("F2") + vel.ToString("F2") + "\n";

        // Envía la cadena de datos al Raspberry Pi Pico a través de la comunicación serial
        if (serialPort.IsOpen)
        {
            serialPort.Write(dataToSend);
        }

        if (serialPort.BytesToRead > 0)
        {
            // Lee la respuesta y guárdala en una variable (en este ejemplo, simplemente mostramos la respuesta)
            string response = serialPort.ReadLine();
            Debug.Log("Respuesta del Raspberry Pi Pico: " + response);
        }
    }

    private void OnDestroy()
    {
        // Cierra el puerto serial al salir de la aplicación
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
