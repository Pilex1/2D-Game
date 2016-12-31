#version 400 core

in vec2 vert_pos;
in vec2 vert_uv;

in vec3 vert_lighting;

uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

out vec2 frag_uv;
out vec3 frag_lighting;

void main(void) {
	gl_Position = projectionMatrix * viewMatrix * vec4(vert_pos, 0.0, 1.0);
	frag_uv = vert_uv;
	frag_lighting = vert_lighting;
}