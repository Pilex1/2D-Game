#version 400 core

in vec2 frag_uv;
in vec3 frag_lighting;

uniform sampler2D texture;

const float ambientLighting = 0.1;

out vec4 fragment;

void main(void) {
	vec4 tex = texture2D(texture, frag_uv);
	if (tex.w == 0) discard;
	fragment.xyz = frag_lighting * tex.xyz;
	fragment.w = tex.w;
}