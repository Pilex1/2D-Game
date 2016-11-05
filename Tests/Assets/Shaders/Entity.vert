#version 410 core

in vec2 vpos;

uniform vec2 pos;
uniform vec2 size;

void main(void) {
	gl_Position = vec4(size * vpos + pos, 0.0, 1.0);
}