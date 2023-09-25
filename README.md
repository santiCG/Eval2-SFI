# ef_un2

## Manual del usuario 
> Bienvenidos al “Escape del Himalaya”. Si desean sobrevivir, deben alcanzar la puerta de salida pueda abrirse,  resolver diferentes acertijos, y encontrar los códigos para superar cada desafío. Oh, pero eso no es todo, los acertijos se harán más y más difíciles a medida que los sobrelleves y el tiempo es limitado. Apresurate y escapa del Himalaya.
### Instrucciones
El programa técnico iniciará desde una configuración de 4 variables (Temperatura, presión, altura, velocidad). La temperatura afectará la cantidad de acertijos totales además de un tiempo invisible que será considerado como el "límite", mientras que presión y altura son características personalizables (aunque en la propia programación no se aprecie este cambio). Velocidad es el cambio por una cantidad determinada de tiempo en que cambian las cosas, es decir, los cambios que se apreciarán en el funcionamiento del programa como lo son los acertijos resueltos, la temperatura y la dificultad progresiva.

Como esto es una prueba técnica los usuarios no interaccionarán en teoría con el programa. En su lugar podrán observar los cambios apreciables con las variables que se configuraron en un inicio, en especial temperatura y velocidad. Se establecerá una función límite que haga correr el programa para que no continúe indefinidamente.

#### Datos extra
Estas variables se configurarán a través del Raspberry Pi Pico para mandarlos a la interfaz de Unity como medio de muestra de dicha función.

# Manual del desarrollador

> Este explicará a grandes rasgos la funcionalidad de los códigos empleados para el uso a futuro de otros desarrolladores que entiendan la funcionalidad de cada elemento empleado 

## para Unity
### Código - Botón 
#### Variables
``` c#
    	public Slider temperaturaSlider;
    	public Slider altitudSlider;
    	public Slider presionSlider;
    	public Text acertijosResueltosText;
    	public Text acertijosTotalesText;
    	public Text nivelDificultadText;

    	public Camera camaraEscena;
    	public GameObject CarpetadeSliders;

    	public Button boton;
```

- Public Slider: es el objeto que asocia las barras deslizadoras que ayudarán a configurar las diferentes variables antes de entrar en el Escape Room.
- Public Text: es el texto que se asignará para decir el nombre de la variable y la cantidad en el momento (aunque este código en específico no lo indique).
- Public Camera: es la cámara que se manipulará a lo largo del Unity a nuestra propia conveniencia.
- Public Button: Será el botón que se presionará para terminar de configurar las características del programa.
- Public GameObject: Funcionará como una carpeta madre en la que se guardarán todos los textos, botones y sliders para dejar de mostrarlos más adelante.

#### Funciones
``` c#
    void Start()
    {
   	     boton.onClick.AddListener(cambiarPosicionCamara);
    }

    void cambiarPosicionCamara()
    {
        camaraEscena.transform.position = new Vector3(76, 3, -28);
        CarpetadeSliders.SetActive(false);
    }
```

- Es la función que asignará el botón para que cumpla la función especificada. En este caso se establecerá el Void Start para que en el momento de que se presiona el botón se arremeta al Void cambiarPosicionCamara; siendo este void quien cumple dos funciones: mueve la cámara para mostrar en la escena de Unity la imagen del programa iniciado (los pingüinos) y ocultar los elementos de la configuración base.

### Código - Serial 
#### Variables

``` c#
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

    private SerialPort _serialPort = new SerialPort();
    private byte[] buffer = new byte[32];

    private static int counter = 0;

    private int varTemp;
    private int varPressure;
    private int varHeight;
    private int varSpeed;
```
- Slider y Text se mantiene con la misma funcionalidad que en el código Botón (en este caso es TextMeshPro, pero sigue cumpliendo la misma función de Text).
- Private int varX: Son las variables que asignarán cada una de las instancias que se necesiten. En este caso, para configurar Temperatura, presión, altura y velocidad.
- Private SerialPort: Crea una comunicación que enlaza el microcontrolador con el código.
- Private byte[] buffer: Para almacenar en el búfer una cantidad determinada de bytes, en este caso 32 de longitud.

#### Funciones
``` c#
    void Update()
    {
        varTemp = Convert.ToInt32(temperaturaSlider.value);
        varPressure = Convert.ToInt32(presionSlider.value);
        varHeight = Convert.ToInt32(altitudSlider.value);
        varSpeed = Convert.ToInt32(velocidadSlider.value);

        alturaText.text = "Altura: " + altitudSlider.value.ToString();
        presionText.text = "Presión: " + presionSlider.value.ToString();
        temperaturaText.text = "Temperatura: " + temperaturaSlider.value.ToString();
        acertijosTotalesText.text = "Acertijos Totales: " + temperaturaSlider.value.ToString();
        velocidadText.text = "Velocidad: " + velocidadSlider.value.ToString();
```
En esta primera parte se cumplen dos objetivos:
1. las instancias que se configuran en los sliders se vuelven variables numéricas con el Convert.ToInt32, en este caso de int.
2. mostrar en el texto el valor de los valores en los sliders, además de complementarlo con un pequeño texto para que se identifique cada cambio.

``` c#
if (Input.GetKeyDown(KeyCode.A))
        {
            byte[] data = { 0x31 };            
            _serialPort.Write(data, 0, 1);
            int numData = _serialPort.Read(buffer, 0, 20);
        }

        if (_serialPort.IsOpen)
        {
            _serialPort.WriteLine("SolicitarDatos");
            string respuesta = _serialPort.ReadLine();

            string[] datos = respuesta.Split(',');

            if (datos.Length == 3)
            {
                float temperaturaActual = float.Parse(datos[0]);
                float altitudActual = float.Parse(datos[1]);
                float presionActual = float.Parse(datos[2]);

                // Actualizar los valores de los Sliders.
                temperaturaSlider.value = temperaturaActual;
                altitudSlider.value = altitudActual;
                presionSlider.value = presionActual;

                // Descenso de temperatura.
                if (temperaturaSlider.value > 0)
                {
                    temperaturaSlider.value -= velocidadDescensoTemperatura * Time.deltaTime;
                }
            }
}
```
1. Se configura que, al ser presionada la letra "A" se ejecuten una serie de instrucciones: se crea un array de datos llamado data que solo contiene un valor, en este caso 0x31 ó 1. Se envían los datos al puerto serial y luego se intenta leer hasta 20 bytes de datos desde el dispositivo y los almacena en un búfer. Esto se hace de modo que el procesamiento sea igual en cualquier dispositivo independiente de su capacidad.
2. Luego se verifica si el puerto serial está abierto, envía una solicitud al dispositivo a través del puerto serial y se incita a las personas que digiten datos por el "String Respuesta", lee la respuesta del dispositivo y luego divide la respuesta en valores utilizando comas como delimitadores (para que en cada coma sea una variable diferente).
3. Al ser comparado con la cantidad de datos ingresados entre comas (exactamente 3), se creará un array que se guardarán las respuestas dependiendo del orden estipulado por el usuario que se modifican para poner temperatura, presión y altura.
4. Se actualizan los valores de los sliders
5. Se desciende la temperatura siempre y cuando el valor del slider de temperatura sea mayor que 0.

## para Arduino

### Bibliotecas e Inclusión de constantes

1. **Arduino.h**
Es una biblioteca estándar que proporciona las funciones y definiciones básicas para programar en Arduino. 

2. **#define LED_PIN 25**
Esto define una constante llamada **LED_PIN**, que se utiliza para especificar el número de pin que controla un LED. En este caso, se asigna el valor 25 a esta constante.
  </li>
</ol>

### Variables
``` c++
float temperature = 0.0;
float altitude = 0.0;
float pressure = 0.0;
``` 
Estas variables se utilizan para almacenar los valores de temperatura, altitud y presión. Inicialmente, se establecen en 0.0.

``` c++
float temperatureChangeSpeed = 0.0;
```
La variable booleana llamada **temperatureChangeSpeed** determina la velocidad de cambio de la temperatura.
``` c++
bool changeTemperature = true;
``` 
y con una bandera booleana llamada **changeTemperature** se almacena la velocidad de cambio de la temperatura.
``` c++
void setup() {
    // Inicializa la comunicación serial
    Serial.begin(9600);

    // Inicializa el pin del LED
    pinMode(LED_PIN, OUTPUT);
}

``` 
La función **setup()** se ejecuta una vez al inicio del programa. Aquí se realizan dos tareas:

Se inicia la comunicación serial a una velocidad de 9600 baudios (bps).
Se configura el pin **LED_PIN** como una salida.
``` c++
// Bucle principal
void loop() {
    // Encender/apagar el LED cada segundo
    digitalWrite(LED_PIN, HIGH);
    delay(500);
    digitalWrite(LED_PIN, LOW);
    delay(500);

// Actualizo la variable de temperatura 
    if (changeTemperature) {
        temperature += temperatureChangeSpeed;
    }

// Verifico si hay datos disponibles en la comunicación serial
    if (Serial.available()) {
    if(Serial.available())
    {
        if(Serial.read() == '1')
        {
            delay(3000);
// Envío un mensaje de respuesta a través de la comunicación serial
            Serial.print("Hello from Raspi");
        }
    }
}
``` 
La función **loop()** es un bucle que se ejecuta continuamente después de que setup() se haya completado. Aquí se llevan a cabo las siguientes acciones:

Enciende y apaga el LED cada 500 milisegundos, creando un efecto de parpadeo.
Actualiza la variable temperature sumando el valor de temperatureChangeSpeed si changeTemperature es verdadero.
Verifica si hay datos disponibles en la comunicación serial. Si se recibe el carácter '1' a través de la comunicación serial, el Arduino espera 3 segundos y luego envía "Hello from Raspi" a través de la comunicación serial.
