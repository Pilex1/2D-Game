#version 410 core

uniform vec4 clr;

out vec4 fragment;

void main(void) {
	fragment = clr;
	if (fragment.w == 0) discard;
}