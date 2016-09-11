#version 400 core

in vec2 vertexPosition;

in vec4 colour;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

out vec4 outColour;

void main(void) {
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertexPosition, 0.0, 1.0);
	outColour = colour;
}