#version 400 core

in vec2 outUV;
in vec4 outColour;

uniform sampler2D texture;
uniform bool useTexture;

out vec4 fragment;

void main(void) {
	if (useTexture) {
		fragment = texture2D(texture, outUV);
		if (fragment.a < 0.5) discard;
	} else {
		fragment = outColour;
	}
}