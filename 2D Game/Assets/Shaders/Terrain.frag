#version 400 core

in vec2 frag_uv;
in vec4 frag_lighting;

uniform sampler2D texture;

const float ambientLighting = 0.1;

out vec4 fragment;

void main(void) {
	vec4 tex = texture2D(texture, frag_uv);
	if (tex.w == 0) discard;
	vec4 lighting = vec4(max(frag_lighting.rgb, vec3(ambientLighting, ambientLighting, ambientLighting)), frag_lighting.a);
	fragment = frag_lighting * tex;
}