
# Make fake graph for data and testing

# Nodes

class Node:
    def __init__(self, nodeID, long, lat):
        self.nodeID = nodeID
        self.long = long
        self.lat = lat

    def nodeID(self):
        return self.nodeID
    def long(self):
        return self.long
    def lat(self):
        return self.lat

class Device:
    def __init__(self, DevID):
        self.DevID = DevID
        self.NodeIn = "Not Set"
        self.NodeOut = "Not Set"
        self.Inflow = True
    def NodeIn(self):
        return self.NodeIn
    def NodeOut(self):
        return self.NodeOut
    def Inflow(self):
        return self.Inflow


