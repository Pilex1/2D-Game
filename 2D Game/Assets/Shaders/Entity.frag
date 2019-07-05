#version 330

precision highp float;
precision highp int;

in vec2 fuv;

uniform sampler2D texture;
uniform vec4 clr;

out vec4 fragment;

void main(void) {
	fragment = texture2D(texture, fuv);
	fragment *= clr;
	if (fragment.w == 0.0) discard;
}