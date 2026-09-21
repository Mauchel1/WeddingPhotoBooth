const byte RELAY_PIN = 2;

void setup()
{
    pinMode(RELAY_PIN, OUTPUT);
    digitalWrite(RELAY_PIN, HIGH);

    Serial.begin(115200);
    delay(1000);
    Serial.println("PHOTOBOX_NANO");
}

void loop()
{
    if (Serial.available())
    {
        String cmd = Serial.readStringUntil('\n');
        cmd.trim();

        if (cmd == "RELAY ON")
        {
            digitalWrite(RELAY_PIN, LOW);
            Serial.println("OK");
        }
        else if (cmd == "RELAY OFF")
        {
            digitalWrite(RELAY_PIN, HIGH);
            Serial.println("OK");
        }
        else if (cmd == "PING")
        {
            Serial.println("PHOTOBOX_NANO");
        }
    }
}