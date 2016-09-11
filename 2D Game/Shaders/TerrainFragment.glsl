#version 400 core

in vec2 uv;

uniform sampler2D texture;

out vec4 fragment;

void main(void) {
	fragment = texture2D(texture, uv);
}