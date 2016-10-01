#version 400 core

in vec2 fpos;
out vec4 frag;

uniform sampler2D texture;

void main(void) {
	//frag = texture2D(texture, fpos);
	frag = vec4(fpos.xyy, 0.1);
}