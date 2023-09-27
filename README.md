|# ef_un2

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
### Variables
``` c++
int temperature = 20;
bool juegoActivo = false;
``` 
Esta variable se usa para asignar un valor a la temperatura base. Además se establece un booleano para indicar el momento en que se inicia el juego/programa.

``` c++
String btnState(uint8_t btnState) {
  if (btnState == HIGH) {
    return "OFF";
  } else
    return "ON";
}
```
El btnState funciona como muestra del estado del botón en el momento. Entendemos a botón como la luz que brilla en el propio Raspberry. Esta solo recibe la señal y determina el momento en que está prendido o apagado mediante la función HIGH, que se asigna al momento de tener activo el Raspberry.

``` c++
void task() {
  enum class TaskStates {
    INIT,
    WAIT_COMMANDS
  };
``` 
Se inicializa una tarea vacía que sirve para gestionar los diferentes estados en los que se encuentra el microcontrolador. Por otro lado, se define un enumerador como TaskStates. Se entiende un enumerador como una lista de valores constantes que se utilizan para representar valores discretos, esto para asignar una cantidad de valores finitos para separarlas de otras variables.
Con el enumerador se inicializan las funciones INIT y WAIT_COMMANDS (uno activo a la vez), el INIT arremete a las configuraciones iniciales del microcontrolador para prepararlo a funcionar mientras el WAIT_COMMANDS es quien espera para recibir comandos y realizar su ejecución. Este último trabaja en simultáneo con el WAIT_COMMANDS del código de Unity para generar la conexión entre ambos.

``` c++
  static TaskStates taskState = TaskStates::INIT;
  constexpr uint8_t led = 25;
  constexpr uint8_t button1Pin = 12;
  constexpr uint8_t button2Pin = 13;
  constexpr uint8_t button3Pin = 32;
  constexpr uint8_t button4Pin = 33;
``` 
- El static TaskStates taskState = TaskStates::INIT tiene como propósito mantener el estado de INIT en el taskState activo.
- Constexpr genera constantes a las variables asignadas.
- uint8_t es el tipo de código, siendo en este caso un entero de 8 bits.
- led se asocia al propio LED enlazado al microcontrolador. Los demás funcionan como la variedad de botones que el programa requiere para su funcionamiento. También se asignan valores numéricos para representar un pin asociado a alguno de los botones.

``` c++
    case TaskStates::WAIT_COMMANDS:
      {
        if (Serial.available() > 0) 
        {
          String command = Serial.readStringUntil('\n');
          if (command == "ledON") 
          {
            String data = Serial.readString();
            juegoActivo = true;
            temperature = data.toInt();
            Juego();

            digitalWrite(led, HIGH);
          }
          else if (command == "ledOFF") 
          {
            juegoActivo = false;

            digitalWrite(led, LOW);
          }
          else if (command == "readBUTTONS")
          {
            Serial.print("boton1: ");
            Serial.print(btnState(digitalRead(button1Pin)).c_str());
            Serial.print(" btn2: ");
            Serial.print(btnState(digitalRead(button2Pin)).c_str());
            Serial.print(" btn3: ");
            Serial.print(btnState(digitalRead(button3Pin)).c_str());
            Serial.print(" btn4: ");
            Serial.print(btnState(digitalRead(button4Pin)).c_str());
            Serial.print('\n');
          }
        }
        break;
      }
      default:
      {
        break;
      }
  }
} 
``` 
- if (Serial.available() > 0) se asocia en el momento en que cualquier valor esté ingresado en el puerto serial del programa.
- String command = Serial.readStringUntil('\n') lee el puerto serial siempre y cuando este no inicie una nueva línea. Esto se hace para que esta línea se guarde en el command y se pueda generar la comunicación.
- ¿Recuerdan al inicio del código con el btnState? Bueno, en caso de que esté en ON, y este sea compatible con el command que esté asignado en el puerto serial en ese momento, se cumplen cuatro funciones: guardar en el data una nueva línea en el serial que representará el cambio de temperatura a lo largo de todo el código, vuelve verdadero el juegoActivo para llamar a una clase más adelante, activa el led con el HIGH e inicializa el Juego().
- En caso de que btnState sea OFF ocasionará que el juegoActivo vuelva a ser falso y el led se apague con el LOW.
- Si el command y el readBUTTONS son iguales leerá el estado de los botones en el momento en que se ejecutó el command.
**NOTA:** hay que recordar que el command y sus asignaciones se realizan desde el código de Unity (C#), designado desde el inicio de este código.

``` c++
void setup() 
{
  task();
}

void loop() 
{
  task();

  if (juegoActivo) 
  {
    Juego();
  }
}
``` 
Se establece el task() desde un principio en el programa, mientras que en el Loop se mantiene el task(), pero también se ejecutará el Juego() siempre y cuando juegoActivo sea verdadero.

``` c++
void Juego() 
{
    if (temperature > 0) 
    {
      delay(1000); // descenso de temperatura cada segundo
      Serial.println(temperature);
      temperature--;
    } 
    else 
    {
      Serial.println("PERDISTE! ESTAS CONGELADO!");
      delay(5000);
      temperature = 20;
      juegoActivo = false; // Juego terminado
    }
}
```
- El Void Juego(), se ha mencionado un par de veces antes. Es quien mide la naturaleza de la temperatura, es decir, su estado actual. Al pasar un tiempo, la variable de temperature descenderá hasta que la condición no se cumpla. 
- En caso de no cumplirse, se iría al else donde se determinaría la pérdida del jugador, reiniciando el bucle con el juegoActivo en false, lo que lleva a reiniciar la mayor parte de lo realizado con anterioridad y se definiría el default de temperatura. 
