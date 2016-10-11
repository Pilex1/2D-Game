#version 400 core

in vec2 vpos;
in vec2 vuv;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

out vec2 fuv;

void main(void) {
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vpos, 0.0, 1.0);
	fuv = vuv;
}