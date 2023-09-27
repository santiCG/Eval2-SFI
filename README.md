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
### Variables

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
```
- Public Slider: es el objeto que asocia las barras deslizadoras que ayudarán a configurar las diferentes variables antes de entrar en el Escape Room.
- Public TextMeshProUGUI: es el texto que se asignará para decir el nombre de la variable y la cantidad numérica (no lo asigna, solo lo muestra).
- Public Camera: es la cámara que se manipulará a lo largo del Unity a nuestra propia conveniencia.
- Public Button: Será el botón que se presionará para terminar de configurar las características del programa.
- Public GameObject: Funcionará como una carpeta madre en la que se guardarán todos los textos, botones y sliders para que se pueda manipular su aparición o desaparición dependiendo de lo que sea conveniente en el momento.
- Private int: Son las variables que asignarán cada una de las instancias que se necesiten. En este caso, para configurar Temperatura, presión, altura, velocidad, dificultad y acertijos resueltos.
- Private float: Son variables que requieren un valor decimal en lugar de un entero fijo. En estas se encuentran la variación de la temperatura, temporizadores y tiempos límites.
- Private SerialPort: Crea una comunicación que enlaza el microcontrolador con el código.
- static TaskStates taskState = TaskStates::INIT: tiene como propósito mantener el estado de INIT en el taskState activo, o al menos como estado inicial.
- Private byte[] buffer: Para almacenar en el búfer una cantidad determinada de bytes, en este caso 32 de longitud.

### Funciones

``` c#
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
```
- Se establece la funcionalidad del botón para que, en el momento que este se presione, se ejecute el cambiarPosiconCamara.
- Algunos valores previamente establecidos adquieren un valor default para funcionar como un estándar a la hora de modificar estas mismas variables, ya sea voluntario o involuntario.
- Se configura el serial port para generar una comunicación con el puerto COM, lo que hace el PortName. BaudRate es quien establece la velocidad de baudios, o la velocidad de comunicación entre puerto y programa. DtrEnable habilita la señal DTR (Data Terminal Ready) en el puerto COM. NewLine dibuja una nueva serie de datos que se utiliza para recolectar elementos y volver a enviar datos a través del puerto serial. Open abre el cuerpo, y por ende la comunicación.
- Es un array llamado buffer con 128 de longitud. Este buffer se crea para almacenar datos y trata de perosnas.

``` c#
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
```
En esta primera parte se cumplen dos objetivos:
-  las instancias que se configuran en los sliders se vuelven variables numéricas con el Convert.ToInt32, en este caso de int.
-  mostrar en el texto el valor de los valores en los sliders, además de complementarlo con un pequeño texto para que se identifique cada cambio.

``` c#
case TaskState.INIT:
                taskState = TaskState.WAIT_COMMANDS;
                Debug.Log("WAIT COMMANDS");
                break;
            case TaskState.WAIT_COMMANDS:
```
El código inicializa los Commands, siempre y cuando los Commands entren en los casos que se muestran a continuación (cada caso también influye en el proceso del código del arduino).

``` c#
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
```
El timerPuzzle y timer Difficulty aumentará a medida que pase el tiempo con la función Time.deltaTime. Anteriormente, se asignó un valor límite para juzgar este específico caso de que lo supere. En cuestión de tiempo, la variable de turno (SolvedPuzzles o Difficulty) aumentará, lo que no solo dibujará en la consola el dato, sino que reiniciará el timer para repetir el proceso de nuevo.

``` c#
if (Input.GetKeyDown(KeyCode.Return))
                {
                    tempInicial = varTemp;
                    _serialPort.Write("ledON\n");
                    _serialPort.Write(varTemp.ToString());
                    Debug.Log("Send ledON");
                }
```
En caso de presionar la letra return (o enter), se guardará la temperatura inicial como una variable de temperatura, a su vez mandaría la señal para prender el led y la comunicación con el microcontrolador por una nueva línea, sujeto a un comprobante del proceso mediante el Debug.Log. Eso quiere decir que guarda el último dato de temperatura.

``` c#
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
```
- En caso de que la letra S sea presionada, se enviará un mensaje al serial port para declarar ledOFF (explicación en el propio código de arduino)
- En caso de que la letra R sea presionada, se enviará un mensaje al serial port para leer los botones del código (explicación en el propio código de arduino)

``` c#
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
```
- En caso de que haya un dato en el puerto serial, esta guarda la información a partir de la respuesta dada en la consola una variable.
- string[] values = response.Split(new char[] { ':', ' ' }), hace que la cadena respond se pueda descomponer en varios delimitadores con el uso de comillas ó comas.
- Los Text se usan para mostrar la interfaz al usuario y mostrar el progreso en dicha variable de variables independientes.

``` c#
            default:
                Debug.Log("State Error");
                break;
```
En caso de defecto, en el que se dibuja un mensaje en la consola para marcar un error en la digitalización de datos, para luego romper la cadena de los switch. 

``` c#
    void cambiarPosicionCamara()
    {
        camaraEscena.transform.position = new Vector3(76, 3, -28);
        menuConfig.SetActive(false);
        hud.SetActive(true);

        configurado = true;

        return;
    }
```
- Es la función que asignará el botón para que cumpla la función especificada. En este caso se establecerá el Void Start para que en el momento de que se presiona el botón se arremeta al Void cambiarPosicionCamara; siendo este void quien cumple tres funciones: mueve la cámara para mostrar en la escena de Unity la imagen del programa iniciado (los pingüinos), deja la configuración verdadera y oculta los elementos de la configuración base.


## para Arduino
### Variables

``` c++
int temperature = 20;
bool juegoActivo = false;
``` 
Esta variable se usa para asignar un valor a la temperatura base. Además se establece un booleano para indicar el momento en que se inicia el juego/programa.

### Funciones

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
Se inicializa una tarea vacía que sirve para gestionar los diferentes estados en los que se encuentra el microcontrolador. Por otro lado, se define un enumerador como TaskStates. Se entiende un enumerador como una lista de valores constantes que se utilizan para representar valores discretos, esto para asignar una cantidad de valores finitos para separarlas de otras variables (o decir que las variables asignadas no corran al mismo tiempo y genere un caos innecesario).

Con el enumerador se inicializan las funciones INIT y WAIT_COMMANDS, el INIT arremete a las configuraciones iniciales del microcontrolador para prepararlo a funcionar mientras el WAIT_COMMANDS es quien espera para recibir comandos y realizar su ejecución. Este último trabaja en simultáneo con el WAIT_COMMANDS del código de Unity para generar la conexión entre ambos.

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
