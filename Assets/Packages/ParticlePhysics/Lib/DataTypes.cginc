#ifndef DATA_TYPES_CGINC
#define DATA_TYPES_CGINC


// Should Also Change this number in ShaderConst.cs
#define CELL_CAPACITY 4
#define COLLIDER_CAPACITY (9 * CELL_CAPACITY)

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
	float2 n;
	float2 t;
	float len;
};

#endif