#version 400 core

in vec3 v_pos;
in vec4 v_colour;

out vec4 f_colour;

uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

void main(void) {
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(v_pos, 1);
	f_colour = v_colour;
}
