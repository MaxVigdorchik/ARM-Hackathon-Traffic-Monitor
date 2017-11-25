import json
# Make fake graph for data and testing

# Nodes

class Node:
    def __init__(self, NodeID, long, lat):
        self.nodeID = NodeID
        self.long = long
        self.lat = lat

    def NodeID(self):
        return self.nodeID
    def long(self):
        return self.long
    def lat(self):
        return self.lat

class Device:
    def __init__(self, DevID, NodeAID, NodeBID, Inflow):
        self.DevID = DevID
        self.NodeAID = NodeAID
        self.NodeBID = NodeBID
        self.Inflow = Inflow
    def DevID(self):
        return self.DevID
    def NodeAID(self):
        return self.NodeAID
    def NodeBID(self):
        return self.NodeBID
    def Inflow(self):
        return self.Inflow

NodeA = Node(1, 0, 10)
NodeB = Node(2, 0, 20)
DeviceA = Device(1, 1, 2, True)
DeviceB = Device(2, 1, 2, False)

nodes = {NodeA.NodeID(): [NodeA.long(), NodeA.lat()]}
devices = {DeviceA.DevID(): [DeviceA.NodeAID(), DeviceA.NodeBID(), DeviceA.Inflow()]}

print(nodes)
print(devices)



