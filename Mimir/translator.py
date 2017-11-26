#!/usr/bin/env python3
import mimir
import paho.mqtt.client as mqtt
import logging

logging.basicConfig()
logger = logging.getLogger(__name__)
logger.setLevel(logging.INFO)


def on_message(cl, userdata, message):
    logger.info("Received message on topic %s", message.topic)
    if message.topic == userdata[1]:
        m_info = cl.publish(userdata[2],
                            mimir.dotnet(mimir.decode(message.payload)), 1)
        logger.info("Translated raw message %d as "
                    "new traffic message %d", message.mid, m_info.mid)


def on_connect(cl, userdata, flags, rc):
    if rc == 0:
        logger.info("Connected to MQTT broker at %s", userdata[0])
    else:
        logger.error("Client failed to connect to %s", userdata[0])
        return RuntimeError("Client failed to connect to %s", userdata[0])


class Translator:
    def __init__(self, host='localhost', raw_topic='Valhalla/Odin/Raw',
                 traffic_topic='Valhalla/Traffic'):
        self.cl = mqtt.Client()
        self.cl.user_data_set((host, raw_topic, traffic_topic))
        self.cl.on_connect = on_connect
        self.cl.on_message = on_message
        logger.info("Initialising %s with host=%s",
                    self.__class__.__name__, host)
        self._host = host
        self._raw = raw_topic
        self._traffic = traffic_topic

    def connect(self):
        logger.info("Connecting to MQTT broker at %s...", self.host)
        self.cl.connect(self.host)

    @property
    def host(self):
        return self._host

    @property
    def raw_topic(self):
        return self._raw

    @property
    def traffic_topic(self):
        return self._traffic


def main():
    # tr = Translator(host='mqtt.ntomi.me')
    # tr.connect()

    cl = mqtt.Client(userdata=['mqtt.ntomi.me', 'Valhalla/Odin/Raw', 'Valhalla/Traffic'])
    cl.on_connect = on_connect
    cl.on_message = on_message
    cl.connect('mqtt.ntomi.me')
    cl.subscribe('Valhalla/Odin/Raw', 1)
    cl.loop_forever()


if __name__ == '__main__':
    main()
