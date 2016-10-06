#version 400 core

in vec2 fuv;

uniform sampler2D texture;
uniform vec3 clr;

out vec4 fragment;

void main(void) {
	fragment = texture2D(texture, fuv);
	fragment.xyz *= clr;
	if (fragment.w == 0) discard;
}