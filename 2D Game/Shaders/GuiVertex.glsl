#version 400 core

in vec2 vertexPosition;

in vec4 colour;

uniform vec2 position;

out vec4 outColour;

void main(void) {
	gl_Position = vec4(position + vertexPosition, 0.0, 1.0);
	outColour = colour;
}