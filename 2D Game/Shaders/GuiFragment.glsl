#version 400 core

in vec2 outUV;

uniform sampler2D texture;

out vec4 fragment;

void main(void) {
	fragment = texture2D(texture, outUV);
	if (fragment.a == 0) discard;
}