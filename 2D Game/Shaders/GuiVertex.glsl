#version 400 core

in vec2 vertexPosition;
in vec2 uv;

out vec2 outUV;

uniform vec2 position;

const vec2 offset = vec2(-1,-1);

void main(void) {
	gl_Position = vec4(offset + position + vertexPosition, 0.0, 1.0);
	outUV = uv;
}