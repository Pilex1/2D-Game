#version 400 core

in vec2 vert_pos;
in vec2 vert_uv;

in vec3 vert_lighting;

uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

out vec2 frag_uv;
out vec3 frag_lighting;

//http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
vec3 hsv2rgb(vec3 c) {
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main(void) {
	gl_Position = projectionMatrix * viewMatrix * vec4(vert_pos, 0.0, 1.0);
	frag_uv = vert_uv;
	frag_lighting = hsv2rgb(vert_lighting);
}