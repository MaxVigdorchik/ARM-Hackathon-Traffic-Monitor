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

NodeList = [Node(1, 0, 10), Node(2, 0, 20)]

# Device1 = Device(1, 1, 2, True)
# Device2 = Device(2, 1, 2, False)

DeviceList = [Device(1, 1, 2, True), Device(2, 1, 2, False)]

nodes = {}
devices = {}

for i in range(len(NodeList)):
    nodes.update({NodeList[i].id: [NodeList[i].long, NodeList[i].lat]})
    devices.update({DeviceList[i].id: [DeviceList[i].node_in, DeviceList[i].node_out, DeviceList[i].inflow]})

print(nodes)
print(devices)



