#version 400 core

in vec2 vertexPosition;

in vec2 uv;
in vec4 colour;

out vec2 outUV;
out vec4 outColour;

uniform vec2 position;

void main(void) {
	gl_Position = vec4(position + vertexPosition, 0.0, 1.0);
	outUV = uv;
	outColour = colour;
}