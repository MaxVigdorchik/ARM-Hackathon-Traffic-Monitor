#ifndef DATATYPES_H
#define DATATYPES_H
#include <vector>
#define CAR_HEIGHT 1000
struct Interaction
{
    time_t start_time;
    float duration;
};

struct Packet
{
    uint32_t id;
    std::vector<Interaction> interactions;
};

#endif /* DATATYPES_H */
