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
    def __init__(self, DevID):
        self.DevID = DevID
        self.NodeAID = "Not Set"
        self.NodeBID = "Not Set"
        self.Inflow = True
    def NodeIn(self):
        return self.NodeIn
    def NodeOut(self):
        return self.NodeOut
    def Inflow(self):
        return self.Inflow

NodeA = Node(1, 0, 10)
DeviceA = Device(1)
# nodes = {'NodeID': NodeA.NodeID, 'Long': NodeA.long, 'Lat': NodeA.lat}
nodes = {NodeA.NodeID: NodeA.long, NodeA.lat}
devices = {}



