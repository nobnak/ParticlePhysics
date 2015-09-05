#ifndef DATA_TYPES_CGINC
#define DATA_TYPES_CGINC

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

#endif