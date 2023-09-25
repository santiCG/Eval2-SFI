#include "Arduino.h"

#define LED_PIN 25 // Asegúrate de ajustar este valor al pin correcto para tu LED

// Variables
float altitude = 500.0;
float pressure = 10.0;
float temperature = 25.0;

// Velocidades de cambio
float temperatureChangeSpeed = 0.0;

// Flags para verificar si las variables deben cambiar
bool changeTemperature = true;

bool juegoActivo = true; // Bandera para indicar si el juego está activo

void setup() {
    // Inicializa la comunicación serial
    Serial.begin(9600);

    // Inicializar el pin del LED
    pinMode(LED_PIN, OUTPUT);
}

void update()
{
  // Espera una instrucción de la aplicación de Unity
    char instruction;
    while (!Serial.available());
    instruction = Serial.read();

    // Si la instrucción es correcta, envía los valores de las variables
    Juego();

    // Escucha datos enviados desde Unity y actualiza las variables
    /*if (Serial.available()) {
        String data = Serial.readStringUntil('\n');
        sscanf(data.c_str(), "%f,%f,%f,%f", &altitude, &pressure, &temperature, &temperatureChangeSpeed);
    }*/

    if(Serial.available()) {
    String message = Serial.readStringUntil('\n'); // Leer el mensaje hasta el final de la línea
    if(message[0] == 'T') {
        temperature = message.substring(1).toFloat(); // Convertir el resto del mensaje a un número y almacenarlo en la variable de temperatura
    }
}

    // Envía el valor actualizado de temperature a Unity cada segundo
    if (juegoActivo) {
        Serial.print('T'); // Marca de inicio de temperatura
        Serial.println(temperature);
        delay(1000);
    }
}

void loop() {
    // Encender/apagar el LED cada segundo
    digitalWrite(LED_PIN, HIGH);
    delay(500);
    digitalWrite(LED_PIN, LOW);
    delay(500);
}

void Juego() 
{
  while (juegoActivo) 
  {
    if (temperature > 0) 
    {
      temperature -= temperatureChangeSpeed;
      Serial.println("Temperatura: " + String(temperature));
      delay(1000); // descenso de temperatura cada segundo
    } 
    else 
    {
      Serial.println("PERDISTE! ESTÁS CONGELADO!");
      delay(5000);
      temperature = 20;
      juegoActivo = false; // Juego terminado
    }
  }
}
