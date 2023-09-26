int temperature = 20;
bool juegoActivo = false;

String btnState(uint8_t btnState) {
  if (btnState == HIGH) {
    return "OFF";
  } else
    return "ON";
}

void task() {
  enum class TaskStates {
    INIT,
    WAIT_COMMANDS
  };
  static TaskStates taskState = TaskStates::INIT;
  constexpr uint8_t led = 25;
  constexpr uint8_t button1Pin = 12;
  constexpr uint8_t button2Pin = 13;
  constexpr uint8_t button3Pin = 32;
  constexpr uint8_t button4Pin = 33;

  switch (taskState) 
  {
    case TaskStates::INIT:
      {
        Serial.begin(115200);
        pinMode(led, OUTPUT);
        digitalWrite(led, LOW);
        pinMode(button1Pin, INPUT_PULLUP);
        pinMode(button2Pin, INPUT_PULLUP);
        pinMode(button3Pin, INPUT_PULLUP);
        pinMode(button4Pin, INPUT_PULLUP);
        taskState = TaskStates::WAIT_COMMANDS;
        break;
      }
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