#version 320 es

precision highp float;
precision highp int;

in vec2 vpos;
in vec2 vuv;

out vec2 fuv;

uniform vec2 position;
uniform vec2 size;
uniform float aspectRatio;

void main(void) {
	vec2 pos = vpos;
	pos.y *= aspectRatio;
	pos *= size;
	pos += position;
	gl_Position = vec4(pos, 0.0, 1.0);
	fuv = vuv;
}