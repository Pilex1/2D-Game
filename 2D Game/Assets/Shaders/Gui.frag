#version 400 core

in vec2 fuv;

uniform sampler2D texture;
uniform vec3 colour;

out vec4 fragment;

//http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main(void) {
	fragment = texture2D(texture, fuv);
	fragment.xyz *= hsv2rgb(colour);
	if (fragment.a == 0) discard;
}

