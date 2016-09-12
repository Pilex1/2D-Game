#version 400 core

in vec2 uv;

in float lighting;

uniform sampler2D texture;

out vec4 fragment;

void main(void) {
	fragment = lighting * texture2D(texture, uv);
}