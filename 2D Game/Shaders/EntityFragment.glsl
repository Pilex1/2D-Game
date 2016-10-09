#version 400 core

in vec2 fuv;

uniform sampler2D texture;
uniform vec4 clr;

out vec4 fragment;

void main(void) {
	fragment = texture2D(texture, fuv);
	fragment *= clr;
	if (fragment.w == 0) discard;
}