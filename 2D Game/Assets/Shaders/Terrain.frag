#version 320 es

precision highp float;
precision highp int;

in vec2 frag_uv;
in vec3 frag_lighting;

uniform sampler2D texture;

out vec4 fragment;

void main(void) {
	vec4 tex = texture2D(texture, frag_uv);
	if (tex.w == 0.0) discard;
	fragment.xyz = frag_lighting * tex.xyz;
	fragment.w = tex.w;
}