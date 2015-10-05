#ifndef DATA_TYPES_CGINC
#define DATA_TYPES_CGINC



#define COLLIDER_CAPACITY 10

struct Wall {
	float2 n;
	float2 t;
	float dn;
	float dt;
	float w;
	float h;
};
struct Collision {
	uint count;
	uint colliders[COLLIDER_CAPACITY];
};

struct Polygon {
	float4 bounds;
	int segmentIndex;
	int segmentCount;
};
struct Segment {
	float2 from;
	float len;
	float2 n;
	float2 t;
};

#endif