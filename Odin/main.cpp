#include "mbed.h"
#include "C12832.h"
#include "OdinWiFiInterface.h"
#include "vl53l0x_api.h"
#include "vl53l0x_platform.h"
#include "vl53l0x_i2c_platform.h"
#include "datatypes.h"
#include "MQTTNetwork.h"
#include "MQTTmbed.h"
#include "MQTTClient.h"
#include "ntp-client/NTPClient.h"

#define USE_I2C_2V8
#define INIT_ID 0 //Set the initial ID here, can be changed with buttons
#define DEBUG_MODE 1 //Debug mode prints out to serial


/* Sets up LCD and prints sensor data value of Indoor Air Quality sensor to LCD */
C12832 lcd(PE_14, PE_12, PD_12, PD_11, PE_9); //LCD: MOSI, SCK, RESET, A0, nCS
Serial pc(USBTX, USBRX);
OdinWiFiInterface wifi;
InterruptIn button(PF_2);
Ticker message_timer; //For sending over measurements
Ticker fake_timer; //For keeping unix time without RTC
Timer t;
volatile bool publish = false;
volatile int message_num = 0;
Packet unsent_data;
volatile time_t current_time;

void lcd_print(const char* message) {
    lcd.cls();
    lcd.locate(0, 3);
    lcd.printf(message);
}

void increment_id()
{
    unsent_data.id = (unsent_data.id + 1) % 32;
    pc.printf("Current id is %u \r\n", unsent_data.id);
    char id_message[15];
    sprintf(id_message, "ID is %u", unsent_data.id);
    lcd_print(id_message);
}

void messageArrived(MQTT::MessageData& md) {
    MQTT::Message &message = md.message;
    char msg[300];
    sprintf(msg, "Message arrived: QoS%d, retained %d, dup %d, packetID %d\r\n", message.qos, message.retained, message.dup, message.id);
    lcd_print(msg);
    wait_ms(1000);
    char payload[300];
    sprintf(payload, "Payload %.*s\r\n", message.payloadlen, (char*)message.payload);
    lcd_print(payload);
}

void publish_message() {
    publish = true;
}

VL53L0X_Error WaitMeasurementDataReady(VL53L0X_DEV Dev) {
    VL53L0X_Error Status = VL53L0X_ERROR_NONE;
    uint8_t NewDatReady=0;
    uint32_t LoopNb;
    
    if (Status == VL53L0X_ERROR_NONE) {
        LoopNb = 0;
        do {
            Status = VL53L0X_GetMeasurementDataReady(Dev, &NewDatReady);
            if ((NewDatReady == 0x01) || Status != VL53L0X_ERROR_NONE) {
                break;
            }
            LoopNb = LoopNb + 1;
            VL53L0X_PollingDelay(Dev);
        } while (LoopNb < VL53L0X_DEFAULT_MAX_LOOP);

        if (LoopNb >= VL53L0X_DEFAULT_MAX_LOOP) {
            Status = VL53L0X_ERROR_TIME_OUT;
        }
    }

    return Status;
}

VL53L0X_Error WaitStopCompleted(VL53L0X_DEV Dev) {
    VL53L0X_Error Status = VL53L0X_ERROR_NONE;
    uint32_t StopCompleted=0;
    uint32_t LoopNb;

    if (Status == VL53L0X_ERROR_NONE) {
        LoopNb = 0;
        do {
            Status = VL53L0X_GetStopCompletedStatus(Dev, &StopCompleted);
            if ((StopCompleted == 0x00) || Status != VL53L0X_ERROR_NONE) {
                break;
            }
            LoopNb = LoopNb + 1;
            VL53L0X_PollingDelay(Dev);
        } while (LoopNb < VL53L0X_DEFAULT_MAX_LOOP);

        if (LoopNb >= VL53L0X_DEFAULT_MAX_LOOP) {
            Status = VL53L0X_ERROR_TIME_OUT;
        }

    }

    return Status;
}

void increment_time()
{
    current_time++;    
}
int main()
{   
    //Setup Wifi
    lcd_print("Connecting...");
    int ret = wifi.connect(MBED_CONF_APP_WIFI_SSID, MBED_CONF_APP_WIFI_PASSWORD, NSAPI_SECURITY_WPA_WPA2);
    if (ret != 0) {
        lcd_print("Connection error.");
        if(DEBUG_MODE) pc.printf("Error code is %d \r\n", ret);
        return -1;
    }
    
    NetworkInterface* net = &wifi;
    NTPClient ntp(net);
    
    time_t timestamp = ntp.get_timestamp();//Get real time 
    if (timestamp < 0) {
            if(DEBUG_MODE) pc.printf("An error occurred when getting the time. Code: %ld\r\n", timestamp);
        } else {
            if(DEBUG_MODE) pc.printf("Current time is %u \r\n", timestamp);
    }
    //set_time(timestamp); //Set RTC to the obtained time. 
    current_time = timestamp; //Turns out there is no rtc.
    fake_timer.attach(&increment_time, 1.0);
    
    //Set up MQQT
    MQTTNetwork mqttNetwork(net);
    MQTT::Client<MQTTNetwork, Countdown> client(mqttNetwork);

    // const char* host = "mqtt.ntomi.me";
    const char* host = "172.21.145.206";
    const char* topic = "Valhalla/Odin/Raw";
    lcd_print("Connecting to MQTT network...");
    int rc = mqttNetwork.connect(host, 1883);
    if (rc != 0) {
        lcd_print("Connection error.");
        return -1;
    }
    lcd_print("Successfully connected!");
    
    MQTTPacket_connectData data = MQTTPacket_connectData_initializer;
    data.MQTTVersion = 3;
    
    char* mac;
    std::memcpy(mac, net->get_mac_address(), 6 * sizeof(char));
    char client_id[20];
    sprintf(client_id, "Client Number %u", INIT_ID);
    data.clientID.cstring = client_id;
    client.connect(data);
    client.subscribe(topic, MQTT::QOS1, messageArrived);

    //Setup laser
    int var=1, measure=0;
    int ave=0, sum=0;
    VL53L0X_Dev_t MyDevice;
    VL53L0X_Dev_t *pMyDevice = &MyDevice;
    VL53L0X_RangingMeasurementData_t    RangingMeasurementData;
    VL53L0X_RangingMeasurementData_t   *pRangingMeasurementData    = &RangingMeasurementData;
    
    // Initialize Comms laster
    pMyDevice->I2cDevAddr      = 0x52;
    pMyDevice->comms_type      =  1;
    pMyDevice->comms_speed_khz =  400;
    
    
    VL53L0X_RdWord(&MyDevice, VL53L0X_REG_OSC_CALIBRATE_VAL,0);
    VL53L0X_DataInit(&MyDevice); 
    uint32_t refSpadCount;
    uint8_t isApertureSpads;
    uint8_t VhvSettings;
    uint8_t PhaseCal;
    
    VL53L0X_StaticInit(pMyDevice); 
    VL53L0X_PerformRefSpadManagement(pMyDevice, &refSpadCount, &isApertureSpads); // Device Initialization
    VL53L0X_PerformRefCalibration(pMyDevice, &VhvSettings, &PhaseCal); // Device Initialization
    VL53L0X_SetDeviceMode(pMyDevice, VL53L0X_DEVICEMODE_CONTINUOUS_RANGING); // Setup in single ranging mode
    VL53L0X_SetLimitCheckValue(pMyDevice, VL53L0X_CHECKENABLE_SIGNAL_RATE_FINAL_RANGE, (FixPoint1616_t)(0.25*65536)); //High Accuracy mode, see API PDF
    VL53L0X_SetLimitCheckValue(pMyDevice, VL53L0X_CHECKENABLE_SIGMA_FINAL_RANGE, (FixPoint1616_t)(18*65536)); //High Accuracy mode, see API PDF
    VL53L0X_SetMeasurementTimingBudgetMicroSeconds(pMyDevice, 200000); //High Accuracy mode, see API PDF
    VL53L0X_StartMeasurement(pMyDevice);
    
    unsent_data.id = INIT_ID;
    message_timer.attach(&publish_message, 10.0); 
    button.fall(&increment_id);
    
    time_t start_time;
    time_t end_time;
    float duration;
    bool is_car = false;
    
    while(1) {
            while(var<=5){ //Take average of a few measurements
                WaitMeasurementDataReady(pMyDevice);
                VL53L0X_GetRangingMeasurementData(pMyDevice, pRangingMeasurementData);
                measure=pRangingMeasurementData->RangeMilliMeter;
                sum=sum+measure;
                VL53L0X_ClearInterruptMask(pMyDevice, VL53L0X_REG_SYSTEM_INTERRUPT_GPIO_NEW_SAMPLE_READY);
                VL53L0X_PollingDelay(pMyDevice);
                var++;
                }
        ave=sum/var;
        var=1;
        sum=0; 
        
        if(DEBUG_MODE) pc.printf("Measurement is %d \r\n", ave);
        if(ave < CAR_HEIGHT && !is_car) //Car initially rolls over sensor
        {
            start_time = current_time;
            if(DEBUG_MODE) pc.printf("start time was %u \r\n", start_time);
            t.start();
            is_car = true;
        } else if(ave >= CAR_HEIGHT && is_car) //Car leaves sensor
        {
            end_time = current_time;
            t.stop();
            duration = t.read();
            t.reset();
            is_car = false;
            
            Interaction temp_data;
            temp_data.start_time = start_time;
            temp_data.duration = duration;
            unsent_data.interactions.push_back(temp_data);
            //TODO: Create the packet to send here!
        }
        if(DEBUG_MODE)
        {
            for(std::vector<Interaction>::iterator it = unsent_data.interactions.begin(); it != unsent_data.interactions.end(); ++it)
            {
                pc.printf("Duration was %f \r\n", (*it).duration);   
            }   
        }

        if (publish && !is_car) {            
            lcd_print("Sending message...");
            MQTT::Message message;
            message.qos = MQTT::QOS1; //This may allow repeats, but has been most reliable with testing
            message.retained = false;
            message.dup = false;
            uint32_t send_array[1 + unsent_data.interactions.size()*2];
            //uint32_t *id_ptr = (uint32_t*) send_array; 
            //*id_ptr = (uint32_t) unsent_data.id;
            send_array[0] = unsent_data.id; //First element for storing ids
                        
            for(int i = 0; i < unsent_data.interactions.size(); i++)
            {
                send_array[2*i+1] = (uint32_t) unsent_data.interactions[i].start_time;    
                send_array[2*i + 2] = *((uint32_t*) &unsent_data.interactions[i].duration);
            }
            message.payload = (void*) send_array;
            message.payloadlen = sizeof(send_array);
            rc = client.publish(topic, message);
            client.yield(1000);
            lcd_print("Sent message");
            publish = false;
            unsent_data.interactions.clear();
        }
        wait(0.1);
    } 
}
