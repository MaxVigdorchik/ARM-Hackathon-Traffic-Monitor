import json
# Make fake graph for data and testing

# Nodes


class Node:
    def __init__(self, id, long, lat):
        self.id = id
        self.long = long
        self.lat = lat


class Device:
    def __init__(self, id, node_in, node_out, inflow):
        self.id = id
        self.node_in = node_in
        self.node_out = node_out
        self.inflow = inflow


# Node1 = Node(1, 0, 10)
# Node2 = Node(2, 0, 20)

NodeList = [Node(1, 0, 10), Node(2, 0, 20), Node(3, 0, 30), Node(4, 10, 30), Node(5, 10, 40), Node(6, 20, 30), Node(7, 10, 20), Node(8, 10, 10)]

# Device1 = Device(1, 1, 2, True)
# Device2 = Device(2, 1, 2, False)

DeviceList = [Device(1, 1, 2, True), Device(2, 1, 2, False), Device(3, 2, 3, True), Device(4, 2, 3, False), Device(5, 3, 4, True), Device(6, 3, 4, False), Device(7, 4, 5, True), Device(8, 4, 5, False), Device(9, 5, 6, True), Device(10, 5, 6, False), Device(11, 6, 7, True), Device(12, 6, 7, False), Device(13, 7, 8, True), Device(14, 7, 8, False), Device(15, 8, 2, True), Device(16, 8, 2, False)]

nodes = {}
devices = {}

for i in range(len(NodeList)):
    nodes.update({NodeList[i].id: [NodeList[i].long, NodeList[i].lat]})
for i in range(len(DeviceList)):
    devices.update({DeviceList[i].id: [DeviceList[i].node_in, DeviceList[i].node_out, DeviceList[i].inflow]})

with open('nodes.json', 'w') as fp:
    json.dump(nodes, fp)

with open('devices.json', 'w') as fp:
    json.dump(devices, fp)



