#version 410 core

in vec2 vpos;
in vec4 vcolour;
out vec4 fcolour;

uniform vec2 vposoffset;
uniform vec2 vsize;

void main(void) {
	gl_Position = vec4(vsize * vpos + vposoffset, 0, 1);
	fcolour = vcolour;
}
