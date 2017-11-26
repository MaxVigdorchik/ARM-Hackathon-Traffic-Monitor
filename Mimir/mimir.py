import json
import struct
import datetime

__author__ = 'Edwin Balani'
__email__ = 'eb677@srcf.net'
__version__ = '0.0.1'


class OdinReading:
    def __init__(self, start_time, duration):
        if isinstance(start_time, int):
            # Convert time since Unix epoch to a datetime
            start_time = datetime.datetime.fromtimestamp(start_time)
        self._start_time = start_time
        self._duration = duration

    @property
    def start_time(self):
        return self._start_time

    @property
    def duration(self):
        return self._duration


class OdinMessage:
    def __init__(self, device_id, readings):
        self._id = device_id
        self._readings = readings

    @property
    def device_id(self):
        return self._id

    @property
    def readings(self):
        return self._readings


def decode(packet):
    l = len(packet)
    if (l-4) % 8 != 0:
        raise ValueError("Malformed packet: byte length should be a multiple"
                         " of 8, plus 4")

    reading_count = (l-4) // 8
    fstring = '< i ' + ' '.join(['i f']*reading_count)
    s = struct.Struct(fstring)
    device_id, *r = s.unpack(packet)

    def pairs(r_list, strict=True):
        if strict:
            assert len(r_list) % 2 == 0
        for i in range(0, len(r_list), 2):
            yield r_list[i], r_list[i+1]

    readings = list(OdinReading(*p) for p in pairs(r))
    return OdinMessage(device_id, readings)


def dotnet(m):
    epoch = datetime.datetime.utcfromtimestamp(0)
    return json.dumps({
                'deviceID': m.device_id,
                'interactions': [{
                    'start': '\\/Date({:d})\\/'.format(
                        int((r.start_time - epoch).total_seconds() * 1000)),
                    'duration': r.duration
                } for r in m.readings]
            })
