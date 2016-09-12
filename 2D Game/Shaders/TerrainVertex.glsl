#version 400 core

in vec2 vertexPosition;
in vec2 vertexUV;

in float vertexLighting;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

out vec2 uv;
out float lighting;

void main(void) {
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertexPosition, 0.0, 1.0);
	uv = vertexUV;
	lighting = vertexLighting;
}