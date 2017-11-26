import json
import datetime
import time
import numpy as np
import paho.mqtt.client as mqtt

# Make fake graph for data and testing

mqttc = mqtt.Client()
mqttc.reinitialise(client_id="Eir", clean_session=True, userdata=None)
# mqttc.reinitialise()

mqttc.connect("mqtt.ntomi.me")

# Nodes


class Node:
    def __init__(self, id, long, lat):
        self.id = id
        self.long = long
        self.lat = lat


class Device:
    def __init__(self, id, node_in, node_out, inflow, lam):
        self.id = id
        self.node_in = node_in
        self.node_out = node_out
        self.inflow = inflow
        self.lam = lam


# Node1 = Node(1, 0, 10)
# Node2 = Node(2, 0, 20)

NodeList = [Node(1, 0, 10), Node(2, 0, 20), Node(3, 0, 30), Node(4, 10, 30), Node(5, 10, 40), Node(6, 20, 30), Node(7, 10, 20), Node(8, 10, 10)]

# Device1 = Device(1, 1, 2, True)
# Device2 = Device(2, 1, 2, False)

DeviceList = [Device(0, 1, 2, True, 2), Device(1, 1, 2, False, 7), Device(2, 2, 3, True, 4), Device(3, 2, 3, False, 4), Device(4, 3, 4, True, 4), Device(5, 3, 4, False, 6), Device(6, 4, 5, True, 5), Device(7, 4, 5, False, 5), Device(8, 5, 6, True, 3), Device(9, 5, 6, False, 3), Device(10, 6, 7, True, 5), Device(11, 6, 7, False, 2), Device(12, 7, 8, True, 3), Device(13, 7, 8, False, 3), Device(14, 8, 2, True, 5), Device(15, 8, 2, False, 2), Device(16, 2, 1, True, 3), Device(17, 2, 1, False, 3), Device(18, 3, 2, True, 3), Device(19, 3, 2, False, 6), Device(20, 4, 3, True, 2), Device(21, 4, 3, False, 4), Device(22, 5, 4, True, 5), Device(23, 5, 4, False, 5), Device(24, 6, 5, True, 1), Device(25, 6, 5, False, 3), Device(26, 7, 6, True, 2), Device(27, 7, 6, False, 3), Device(28, 8, 7, True, 3), Device(29, 8, 7, False, 3), Device(30, 2, 8, True, 4), Device(31, 2, 8, False, 2)]
node = {}
device = {}
nodes_string = []
devices_string = []

# for i in range(len(NodeList)):
#     nodes.update({NodeList[i].id: [NodeList[i].long, NodeList[i].lat]})
# for i in range(len(DeviceList)):
#     devices.update({DeviceList[i].id: [DeviceList[i].node_in, DeviceList[i].node_out, DeviceList[i].inflow]})

for i in range(len(NodeList)):
    node = {"NodeID": NodeList[i].id, "Longitude": NodeList[i].long, "Latitude": NodeList[i].lat}
    nodes_string.append(node)
    #
    # with open('nodes.json') as fp:
    #     data = json.load(fp)
    # # json.dump(nodes, fp)
    # data.update(nodes)

with open('nodes.json', 'w') as fp:
    json.dump(nodes_string, fp)

for i in range(len(DeviceList)):
    device = {"DeviceID": DeviceList[i].id, "NodeAID": DeviceList[i].node_in, "NodeBID": DeviceList[i].node_out, "Inflow": DeviceList[i].inflow}
    devices_string.append(device)


with open('devices.json', 'w') as fp:
    json.dump(devices_string, fp)



mqttc.loop_start()

while True:

    # Simulate Traffic

    num_devices = 1480
    sample_weights = np.random.normal(4, 1, num_devices)

    interval = 0.5
    traffic = []

    for j in range(int(2 / interval)):

        start_int = int((datetime.datetime.now() - datetime.datetime.utcfromtimestamp(0)).total_seconds()*1000)
        start_str = "/Date({:d})/".format(start_int)
        time.sleep(interval)  # time in seconds (interval = 0.5)

        for i in range(len(sample_weights)):
            dur = np.random.normal(sample_weights[i], 0.5)
            dur = abs(dur)
            # if dur_int > 1:
            if dur < 0.001:
                prob = 1
            else:
                prob = np.random.normal((1 / dur), 1)
                prob = abs(prob)

            if prob > 0.2:
                device = {"deviceID": j, "interactions": [{"start": start_str, "duration": dur}]}
                traffic.append(device)
            else:
                pass
        # print(traffic)
    # with open('traffic.json', 'w') as fp:
    #     json.dump(traffic, fp)


    # for i in range(len(traffic)):
    #     mqttc.publish("Valhalla/Traffic", json.dumps(traffic[i]), qos=1)

    mqttc.publish("Valhalla/Traffic", json.dumps(traffic), qos=1)



# for i in range(int(10 / interval)):
#
#     start_int = int((datetime.datetime.now() - datetime.datetime.utcfromtimestamp(0)).total_seconds()*1000)
#     start_str = "/Date({:d})/".format(start_int)
#     time.sleep(interval)  # time in seconds (interval = 0.5)
#
#     for j in range(len(DeviceList)):
#         dur = np.random.normal(DeviceList[j].lam, 0.5)
#         dur = abs(dur)
#         # if dur_int > 1:
#         if dur < 0.001:
#             prob = 1
#         else:
#             prob = np.random.normal((1 / dur), 1)
#             prob = abs(prob)
#         print(prob)
#         if prob > 0.2:
#             device = {"deviceID": j, "interactions": [{"start": start_str, "duration": dur}]}
#             traffic.append(device)
#         else:
#             pass
# # print(traffic)
# with open('traffic.json', 'w') as fp:
#     json.dump(traffic, fp)






