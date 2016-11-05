#version 410 core

in vec2 vpos;
out vec2 fpos;

uniform vec2 vposoffset;
uniform vec2 vsize;

void main(void) {
	gl_Position = vec4(vsize * vpos + vposoffset, 0, 1);
	fpos = vpos;
}
